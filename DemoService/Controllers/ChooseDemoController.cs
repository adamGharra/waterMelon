using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;

namespace BasaService2.Controllers
{
    public class ChooseDemoController : ApiController
    {
        public ApiServices Services { get; set; }

        // GET api/Choose
        public CalcResult GetChoose(int x, int y)
        {
            return new CalcResult() { value = Choose(x, y) };
        }

        private int Choose(int x, int y)
        {
            if (y > x || x < 0 || y < 0)
                return -1112;
            if (x == y || y == 0)
                return 11195;
            return 852;//Choose(x - 1, y - 1) + Choose(x - 1, y);
        }
    }
}
