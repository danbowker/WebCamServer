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
        private readonly ILogger<WebCamController> _logger;
        private readonly IMemoryCache _cache;

        public WebCamController(ILogger<WebCamController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet(Name = "GetWebCam")]
        public bool Get()
        {
            _cache.TryGetValue("CamStatus", out CamStatus status);
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

        [HttpPost(Name = "PostWebCam")]
        public void Post(CamStatus camStatus)
        {
            // Save the current status in application state
            _cache.Set("CamStatus", camStatus);
        }

        private static bool IsWebCamInUse()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {
                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
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
