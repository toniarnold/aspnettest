using apiservice.Model;
using apiservice.Model.Db;
using apiservice.Services;
using asplib.Controllers;
using asplib.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace apiservice.Controllers
{
    [Clsid("10E4F343-877A-422F-A736-4EF32D87D28A")]
    public partial class AccesscodeController : SmcController<AccesscodeContext, AccesscodeContext.AccesscodeControllerState>
    {
        internal IConfiguration _configuration;
        internal AspserviceDbContext _DbContext;
        internal ISMSService _SMSService;

        internal string _pnonenumber;
        internal string _accesscode;
        internal int _attempts;

        public AccesscodeController() : base()
        {
        }

        public AccesscodeController(
                IConfiguration configuration,
                AspserviceDbContext dbContext,
                ISMSService smsService
            ) : base(configuration)
        {
            _configuration = configuration;
            _DbContext = dbContext;
            _SMSService = smsService;
        }

        internal Task SMSAccesscode(string phonenumber)
        {
            _pnonenumber = phonenumber;
            _accesscode = AccesscodeGenerator.New(AspserviceDb.ACCESSCODE_LENGTH);
            _attempts = 0;

            return _SMSService.Send(_pnonenumber, _accesscode);
        }

        internal bool IsValid(string accesscode)
        {
            return accesscode == _accesscode;
        }

        internal void IncrementAttempts()
        {
            _attempts++;
        }

        internal bool HaveMoreAttempts()
        {
            return _attempts < _configuration.GetValue<int>("VerifyAttempts");
        }

        internal void SaveAccesscode()
        {
            if (this.Session == null)
            {
                throw new InvalidOperationException("Requires an existing Main to save an Accesscode entity");
            }
            var accesscodeEntity = new Accesscode()
            {
                Session = (Guid)this.Session,
                Phonenumber = _pnonenumber,
                Accesscode1 = _accesscode
            };
            _DbContext.Accesscode.Add(accesscodeEntity);
            _DbContext.SaveChanges();
        }
    }
}