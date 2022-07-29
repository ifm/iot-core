# ProfileBuilder

Every ProfileBuilder should inherit from the **BaseProfileBuilder**. The BaseProfileBuilder self has the **IProfileBuilder** interface implemented. 

## The IProfileBuilder interface

IProfileBuilder is the interface of the ProfileBuilders. It contains on single methode called **Build**. With the build methode the profile should be build.
This is nessesary, so profiles can be build with the configurable launcher. The build methode shall not take any arguments.

## The BaseProfileBuilder

The BaseProfileBuilder is the base implementation of a ProfileBuilder. The constructor needs a ProfileBuilderConfiguration object.
This object must always contains an iotcore instance and a base address. This address is the parent address in which the profile will be add on.
ProfileBuilders inheriting from the BaseProfileBuilder have access to the iotcore instance and the baseAddress. 
The BaseProfileBuilder is an abstract classe, the build methode is also abstract. 


## ProfileBuilderConfiguration

The ProfileBuilderConfiguration has two arguments: a **Iotcore**-object and a **Address**-object.
There is also a constructor with no arguments, this is only relevant for the xml serialization in a .config file.
The SettingsName is also only for the configuration  of a launcher relevant. The name will not be used by the profile.
In the configuration the SettingName will be used to find the right config for an assembly.


## GenericSectionHandler

The GenericSectionHandler is only for the xml serilazation relevant. It is used by the configurations in the launcher.




## How to generate a new ProfileBuilder

If you want to generate a new ProfileBuilder it should look like this:
			

			public class NewSampleProfileBuilder : BaseProfileBuilder
				{
                
					rotected NewSampleProfileBuilder(ProfileBuilderConfiguration config) : base(config)
					{
						//Code
					}

					public override void Build()
					{
						//Code
					}  
				}

If the ProfileBuilder needs more arguments than the ProfileBuilderConfiguration give, you can generate your own special configuration.


## Special ProfileBuilderConfiguration

It is possible to make a special configuration for a profile. 
This configuration has to inherite fom the ProfileBuilderConfiguration.
It can look like this for example:

                 public class SpecialProfileBuilderConfiguration : ProfileBuilderConfiguration
                 {
                    public string Info1 { get; set; }
                    public int Info2 { get; set; }

                    public SpecialProfileBuilderConfiguration() : base()
                    {
                       
                    }

                    public SpecialProfileBuilderConfiguration(IIoTCore iotCore, string address, string info1, int info2 = 4) : base(iotCore, address)
                    {
                        Info1 = info1;
                        Info2 = info2;
                    }
                 }

Like the ProfileConfiguration an empty construktor is nessesary for the xml serialization.
An SectionHandler like the GenericSectionHandler is not nessesary.
All the properties in the special config have to be serilizable.
But there are some options to add a property which is not serializable.
If you need for example a TimeSpan in the configuration you can write it in the config like this:

            [XmlIgnore]
            public TimeSpan Timeout { get; set; }

            [XmlElement("Timeout")]
            public long TimeoutFromTicks
            {
                get
                {
                   return Timeout.Ticks;
                }
                set
                {
                    Timeout = TimeSpan.FromTicks(value);
                }
            }

The special configuration has to be in the same project as the ProfileBuilder itself.



















