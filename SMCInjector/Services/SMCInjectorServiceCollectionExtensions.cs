using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;
using SMCInjector.Core.Models;
using System.ComponentModel;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SMCInjectorServiceCollectionExtensions
    {
        private static ICollection<ServiceModel> ServiceModels { get; set; }
        private static ICollection<ConstructorModel> ConstructorModels { get; set; }
        private static ICollection<MappingModel> MappingModels { get; set; }

        public static IServiceCollection AddSmcInjection(this IServiceCollection services, string FilePath)
        {
            LoadConfig(FilePath);
            BeginInjection(services);

            return services;
        }

        private static void LoadConfig(string FilePath)
        {
            using (var confStream = new StreamReader(File.OpenRead(FilePath), Encoding.UTF8))
            {
                var config = new XmlDocument();
                config.Load(confStream);
                
                var services = config.GetElementsByTagName("Services").Item(0) as XmlNode;
                var mappings = config.GetElementsByTagName("Mappings").Item(0) as XmlNode;
                var constructors = config.GetElementsByTagName("Constructors").Item(0) as XmlNode;

                LoadServices(services.ChildNodes);
                LoadMappings(mappings.ChildNodes);
                LoadConstructors(constructors.ChildNodes);
            }
        }

        private static void LoadServices(XmlNodeList Registries)
        {
            ServiceModels = new LinkedList<ServiceModel>();
            foreach (XmlNode registry in Registries)
            {
                var myAssembly = Assembly.Load(new AssemblyName(registry.Attributes["Assembly"].Value));

                if (myAssembly == null)
                    throw new SMTException(String.Format("Assembly {0} could not be found", registry.Attributes["Assembly"].Value));

                var myType = myAssembly.GetType(registry.Attributes["Class"].Value);

                if (myType == null)
                    throw new SMTException(String.Format("Type {0} could not be found in the assembly", registry.Attributes["Class"].Value));

                ServiceModels.Add(new ServiceModel
                {
                    Alias = registry.Attributes["Alias"].Value,
                    Assembly = myAssembly,
                    Class = myType
                });
            }
        }

        private static void LoadMappings(XmlNodeList Mappings)
        {
            MappingModels = new LinkedList<MappingModel>();
            foreach (XmlNode mapping in Mappings)
            {
                MappingModels.Add(new MappingModel
                {
                    InjectionType = mapping.Attributes["InjectionType"].Value,
                    Name = mapping.Attributes["Name"].Value,
                    From = ServiceModels.First(x => x.Alias.Equals(mapping.Attributes["From"].Value)),
                    To = ServiceModels.First(x => x.Alias.Equals(mapping.Attributes["To"].Value))
                });
            }
        }

        private static void LoadConstructors(XmlNodeList Constructors)
        {
            ConstructorModels = new LinkedList<ConstructorModel>();
            foreach (XmlNode constructor in Constructors)
            {
                ICollection<ParamModel> ctorParams = new LinkedList<ParamModel>();
                foreach (XmlNode ctorParam in constructor.ChildNodes)
                {
                    ctorParams.Add(new ParamModel
                    {
                        Name = ctorParam.Attributes["Name"].Value,
                        Value = ctorParam.Attributes["Value"].Value
                    });
                }

                var toAdd = new ConstructorModel
                {
                    MapName = constructor.Attributes["MapName"].Value,
                    Params = ctorParams.Select(x => x).ToList()
                };

                ConstructorModels.Add(toAdd);
            }
        }

        private static void BeginInjection(IServiceCollection services)
        {
            foreach (var mapping in MappingModels)
            {
                if (mapping.From == null || mapping.To == null)
                    throw new SMTException(String.Format("Service mapping is invalid: {0}", mapping.Name));

                switch (mapping.InjectionType.ToLower())
                {
                    case "scoped":
                        services.AddScoped(mapping.From.Class, mapping.To.Class);
                        break;
                    case "singleton":
                        var instance = CreateSMTInstance(mapping);
                        if (instance != null)
                            services.AddSingleton(mapping.From.Class, instance);
                        else
                            services.AddSingleton(mapping.From.Class, mapping.To.Class);
                        break;
                    case "transient":
                    default:
                        services.AddTransient(mapping.From.Class, mapping.To.Class);
                        break;
                }
            }
        }

        private static object CreateSMTInstance(MappingModel model)
        {
            var constructorModel = ConstructorModels.FirstOrDefault(cm => cm.MapName.ToLower().Equals(model.Name.ToLower()));
            if (constructorModel == null)
                return null;

            var definedConstructor = model.To.Class.GetConstructors().FirstOrDefault(ctr => ctr.GetParameters().Length == constructorModel.Params.Count);
            if (definedConstructor == null)
                throw new SMTException(String.Format("No constructor found matching the config file params for this mapping: {0}", model.Name));

            var constParams = definedConstructor.GetParameters();
            var finalParams = new List<object>();

            foreach (var param in constParams)
            {
                var smtParam = constructorModel.Params.FirstOrDefault(p => p.Name.Equals(param.Name));

                if (smtParam == null && !param.HasDefaultValue)
                    throw new SMTException(String.Format("Constructor {0} has no param named {1} or has a default value", definedConstructor.Name, param.Name));

                var shouldUserDefaultValue = smtParam == null && param.HasDefaultValue;
                var equalTypes = param.ParameterType.Equals(typeof(string));
                object paramValue = smtParam == null && param.HasDefaultValue ?
                                        param.DefaultValue :
                                        equalTypes ?
                                            smtParam.Value :
                                            TypeDescriptor.GetConverter(param.ParameterType).ConvertFrom(smtParam.Value);

                finalParams.Add(paramValue);
            }

            return definedConstructor.Invoke(finalParams.ToArray());
        }
    }
}
