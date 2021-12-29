using Discord.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section136Callouts.UpdateAnnouncer.Net6
{
    internal class WebhookHelper
    {

        static WebhookHelper i;
        internal static WebhookHelper Instance => i ?? (i = new WebhookHelper());

        private Webhook webhook;
        

        public WebhookHelper()
        {
            webhook = new Webhook()
            {
                WebhookUrl = "https://discord.com/api/webhooks/925541212344184943/xHPSO0EKvuLFA8QLy94Z-o_j_0Dp-n49M70t0hlRnTuEW-pW6IZV5KL1VHNOaY5b88KN",
                Username = "Section136Callouts Update Detector",
                Message = "A new update to Section136Callouts has been detected"
            };
        }

        public bool SendUpdate(string newVersion, string fullVersionName, out string? failReason)
        {
            Console.WriteLine("[WEBHOOK] Sending update to webhook.");

            failReason = null;

            if (webhook is null)
            {
                failReason = "The webhook is not setup.";
                return false;
            }

            webhook.Message = string.Format("**NEW UPDATE DETECETD!**\n\nSection136Callouts version **{0}** has been released on the LCPDFR website!\n\nYou can download it at https://section136.maxplayledev.org  \n\nVersion information: {1}\nRelease timestamp: Within the last 5 minutes", newVersion, fullVersionName);

            webhook.SendInstance();

            return true;
        }
    }
}
