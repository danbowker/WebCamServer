using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Win32;
using System.Runtime.Serialization;

namespace WebCam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebCamController : ControllerBase
    {
        private readonly IMemoryCache cache;

        public WebCamController(IMemoryCache cache)
        {
            this.cache = cache;
        }

        [HttpGet(Name = "GetWebCam")]
        public bool Get()
        {
            cache.TryGetValue("CamStatus", out CamStatus status);
            switch (status)
            {
                case CamStatus.Auto:
                    return IsWebCamInUse();
                case CamStatus.On:
                    return true;
                case CamStatus.Off:
                    return false;
                default:
                    return false;
            }
        }

        [HttpGet("ClientApp", Name = "GetClientApp")]
        public string? GetClientApp()
        {
            return GetAppUsingWebCam();
        }

        [HttpPost(Name = "PostWebCam")]
        public void Post(CamStatus camStatus)
        {
            // Save the current status in application state
            cache.Set("CamStatus", camStatus);
        }

        private static bool IsWebCamInUse()
        {
            return !string.IsNullOrEmpty(GetAppUsingWebCam());
        }

        private static string? GetAppUsingWebCam()
        {
            return GetAppUsingWebCam(@"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged")
                   ?? GetAppUsingWebCam(@"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam");
        }

        private static string? GetAppUsingWebCam(string keyName)
        {
            using var key = Registry.CurrentUser.OpenSubKey(keyName);
            if (key != null)
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey != null && subKey.GetValueNames().Contains("LastUsedTimeStop"))
                    {
                        var endTime = (long)(subKey.GetValue("LastUsedTimeStop") ?? -1);
                        if (endTime <= 0)
                        {
                            return subKeyName;
                        }
                    }
                }

            return null;
        }
    }
    
    public enum CamStatus
    {
        [EnumMember(Value = "Auto")]
        Auto,
        [EnumMember(Value = "On")]
        On,
        [EnumMember(Value = "Off")]
        Off
    }
}
