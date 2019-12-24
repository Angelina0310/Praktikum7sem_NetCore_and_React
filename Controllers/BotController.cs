using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KDays.Data;
using KDays.Models;
using System.IO;
using VkNet;
using VkNet.Model;
using VkNet.Exception;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

namespace KDays.Controllers
{
    public class BotController
    {
        public static string MyAppToken => "9ead8aaf5203fed3ba7ad08e7824a26d5642261ef1************************************";
        public static ulong MyGroupId => 111111111;
        public static async void StartBot()
        {

            BotWorker();
            //await Task.Run(() => BotWorker());
        }

        public static void BotWorker()
        { 
            var api = new VkApi();
            api.Authorize(new ApiAuthParams() { AccessToken = MyAppToken });
            var s = api.Groups.GetLongPollServer(MyGroupId);
            while (true)
            {
                try
                {
                    var poll = api.Groups.GetBotsLongPollHistory(
                                          new BotsLongPollHistoryParams()
                                          { Server = s.Server, Ts = s.Ts, Key = s.Key, Wait = 1 });
                    if (poll?.Updates == null) continue;
                    foreach (var a in poll.Updates)
                    {
                        if (a.Type == GroupUpdateType.MessageNew)
                        {
                            /*string NextKD = getNextKD(a.Message.Body);
                            Console.WriteLine(a.Message.Body);

                            api.Messages.Send(new MessagesSendParams()
                            {
                                UserId = a.Message.UserId,
                                Message = a.Message.Body
                            });*/
                        }

                    }

                }
                catch (LongPollException exception)
                {
                    if (exception is LongPollOutdateException outdateException)
                        s.Ts = outdateException.Ts;
                    else
                    {
                        s = api.Groups.GetLongPollServer(MyGroupId);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        /*public static string getNextKD(string user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<DateTime> Date = new List<DateTime>();
            IEnumerable<Days> days = db.Days.Where(e => e.UserID == user);
            if(days.Count == 0)
            {
                return "Информации нет(";
            }
            foreach (Days day in days)
            {
                DateTime date = Convert.ToDateTime(day.KDStart);
                Date.Add(date);
            }
            Date.Sort();
            DateTime next = Date[Date.Count - 1];
            next = next.AddMonths(1);
            List<string> returnClass = new List<string>();
            returnClass.Add(next.Date.Day + "." + next.Date.Month + "." + next.Date.Year);
            return returnClass;
        }*/
    }
}