using System;

[assembly: Unity.AutoRegistration.TargetFrameworkInformation(
#if NETSTANDARD1_6
    Unity.AutoRegistration.TargetFramework.netstandard1_6
#elif NETSTANDARD2_0
    Unity.AutoRegistration.TargetFramework.netstandard2_0
#elif NET40
    Unity.AutoRegistration.TargetFramework.net40
#elif NET45
    Unity.AutoRegistration.TargetFramework.net45
#else
    Unity.AutoRegistration.TargetFramework.Unknown
#endif
    )]
namespace Unity.AutoRegistration
{
    
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public enum TargetFramework
    {
        Unknown,
        netstandard1_6,
        netstandard2_0,
        net40,
        net45
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TargetFrameworkInformationAttribute : Attribute
    {

        public TargetFramework TargetFramework { get; }

        public TargetFrameworkInformationAttribute(TargetFramework targetFramework)
        {
            TargetFramework = targetFramework;
        }
        
    }

}
