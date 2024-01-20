using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace WebCam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebCamController : ControllerBase
    {
        private readonly ILogger<WebCamController> _logger;

        public WebCamController(ILogger<WebCamController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWebCam")]
        public bool Get()
        {
            return IsWebCamInUse();
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
}
