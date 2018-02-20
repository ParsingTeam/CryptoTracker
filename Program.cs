using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NetTelegramBotApi;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using NetTelegramBotApi.Types;
using NetTelegramBotApi.Requests;

namespace CryptoCurrency
{
    class Program
    {
        internal static readonly string Token = ""; //Token Here
        private static string BTC = "";
        private static string BTC_Charge = "";
        private static string ETH = "";
        private static string ETH_Charge = "";
        private static string XRP = "";
        private static string XRP_Charge = "";
        private static string BCH = "";
        private static string BCH_Charge = "";
        private static string LTC = "";
        private static string LTC_Charge = "";
        private static string DASH = "";
        private static string DASH_Charge = "";
        private static bool OneTime = false;
        private static ReplyKeyboardMarkup mainMenu;
        static void Main(string[] args){
            mainMenu = new ReplyKeyboardMarkup
            {
                Keyboard = new[] { new[] { "Bitcoin", "Bitcoin Cash","Ethereum" }, new[] { "Litecoin", "Ripple", "Dash" } }
            };
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[" + DateTime.Now + "]" + " | " +"Get First Time Currency Update.");
            UpdateTime();
            var Update = new Thread(UpdateTime); Update.Start();
            Task.Run(() => MainLoop());
        }
        static string Pars(string PainText, bool Mode){
            try{
                switch (Mode){
                    case false:
                        PainText = System.Text.RegularExpressions.Regex.Split(PainText, "price_usd\":")[1].Split(',')[0];
                        if (PainText.Length - PainText.IndexOf(".") == (1 + 1)){
                            return PainText.Replace(" ", "") + "0";
                        }
                        else if (PainText.Length - PainText.IndexOf(".") == (2 + 1)){
                           return PainText.Replace(" ", "");
                        }
                        else if (PainText.IndexOf(".") == -1)
                        {
                            return PainText;
                        }
                        else{
                            return PainText.Substring(0, PainText.IndexOf(".") + (2 + 1)).Replace(" ", "");
                        }
                    default:
                        PainText = System.Text.RegularExpressions.Regex.Split(PainText, "percent_change_1h\":")[1].Split(',')[0];
                        return PainText.Replace(" ", "");
                }
            }
            catch (Exception){
            }
            return "";
        }
        static void UpdateTime(){
            string URL = "https://api.turcan.de/cryptocoins/index.php?coinmarketcap&cryptocoin=";
            try{
                do{
                    string Temp = "";
                    using (var Client = new WebClient()){
                        Temp = Client.DownloadString(URL + "BTC");
                        BTC = Pars(Temp, false);
                        BTC_Charge = Pars(Temp, true);
                        Temp = Client.DownloadString(URL + "ETH");
                        ETH = Pars(Temp, false);
                        ETH_Charge = Pars(Temp, true);
                        Temp = Client.DownloadString(URL + "XRP");
                        XRP = Pars(Temp, false);
                        XRP_Charge = Pars(Temp, true);
                        Temp = Client.DownloadString(URL + "BCH");
                        BCH = Pars(Temp, false);
                        BCH_Charge = Pars(Temp, true);
                        Temp = Client.DownloadString(URL + "LTC");
                        LTC = Pars(Temp, false);
                        LTC_Charge = Pars(Temp, true);
                        Temp = Client.DownloadString(URL + "DASH");
                        DASH = Pars(Temp, false);
                        DASH_Charge = Pars(Temp, true);
                    }
                    if (!OneTime){
                        OneTime = true;
                        return;
                    }
                    Thread.Sleep(60 * 1000);
                    Console.WriteLine("[" + DateTime.Now + "]" + " | " + "Database Updated.");
                } while (true);
            }
            catch (Exception)
            {
            }
        }
        static async Task MainLoop(){
            var bot = new TelegramBot(Token);
            var me =  await bot.MakeRequestAsync(new GetMe());
            Console.WriteLine("[" + DateTime.Now + "]" + " | " + "Bot: [" + me.Username + "] Started.");
            long offset = 0;
            while (true){
                try
                {
                    var updates = await bot.MakeRequestAsync(new GetUpdates() { Offset = offset });
                    foreach (var update in updates)
                    {
                        string Text = update.Message.Text;
                        string User = update.Message.From.Username;
                        if (Text.Length > 15 -1 ){
                            Text = Text.Substring(0, 15) + "...";
                        }
                        if (User == null){
                            User = "None";
                        }
                        Console.WriteLine("[" + DateTime.Now + "]" + " | " + "From: [" + update.Message.From.Id + "]" + " UserName: [" + User + "]" + " Text: " + Text);
                        offset = update.UpdateId + 1;
                        switch (update.Message.Text){
                            case "/start":
                                var Request = new SendMessage(update.Message.Chat.Id, "Welcome to real-time crypto price tracker bot, You can find out at the latest price by choosing any of the following cryptocurrencies." + Environment.NewLine + "Last news at @ParsingTeam") { ReplyMarkup = mainMenu };
                                await bot.MakeRequestAsync(Request);
                                break;
                            default:
                                Request = new SendMessage(update.Message.Chat.Id, PikMessage(update.Message.Text)) { ReplyMarkup = mainMenu };
                                await bot.MakeRequestAsync(Request);
                                break;
                        }
                    }
                    await Task.Delay(500);
                }
                catch (Exception){
                    continue;
                }        
            }
        }
        static string PikMessage(string Currency){
            string Temp =
            "["
            + Currency
            + "]"
            + Environment.NewLine
            + "Price : ";
            switch (Currency){
                case "Bitcoin":
                    Temp = Temp 
                           + BTC
                           + " (USD) ";
                    if (BTC_Charge.Substring(0, 1) == "-"){
                        Temp = Temp
                               + "[Down]"
                               + " %"
                               + BTC_Charge;
                    }
                    else if (BTC_Charge == "0.0" || BTC_Charge == "0")
                    {
                        Temp = Temp
                               + "[Static]"
                               + " %"
                               + BTC_Charge;
                    }
                    else {
                        Temp = Temp
                        + "[UP]"
                        + " %"
                        + BTC_Charge;
                    }
                    return Temp;

                    case "Bitcoin Cash":
                    Temp = Temp
                       + BCH
                       + " (USD) ";
                    if (BCH_Charge.Substring(0, 1) == "-"){
                        Temp = Temp
                               + "[Down]"
                               + " %"
                               + BCH_Charge;
                    }
                    else if (BCH_Charge == "0.0" || BCH_Charge == "0")
                    {
                        Temp = Temp
                               + "[Static]"
                               + " %"
                               + BCH_Charge;
                    }
                    else
                    {
                        Temp = Temp
                        + "[UP]"
                        + " %"
                        + BCH_Charge;
                    }
                    return Temp;

                case "Ethereum":
                    Temp = Temp
                       + ETH
                       + " (USD) ";
                    if (ETH_Charge.Substring(0, 1) == "-"){
                        Temp = Temp
                               + "[Down]"
                               + " %"
                               + ETH_Charge;
                    }
                    else if (ETH_Charge == "0.0" || ETH_Charge == "0")
                    {
                        Temp = Temp
                               + "[Static]"
                               + " %"
                               + ETH_Charge;
                    }
                    else
                    {
                        Temp = Temp
                        + "[UP]"
                        + " %"
                        + ETH_Charge;
                    }
                    return Temp;

                case "Litecoin":
                    Temp = Temp
                       + LTC
                       + " (USD) ";
                    if (LTC_Charge.Substring(0, 1) == "-"){
                        Temp = Temp
                               + "[Down]"
                               + " %"
                               + LTC_Charge;
                    }
                    else if (LTC_Charge == "0.0" || LTC_Charge == "0")
                    {
                        Temp = Temp
                               + "[Static]"
                               + " %"
                               + LTC_Charge;
                    }
                    else
                    {
                        Temp = Temp
                        + "[UP]"
                        + " %"
                        + LTC_Charge;
                    }
                    return Temp;

                case "Ripple":
                    Temp = Temp
                       + XRP
                       + " (USD) ";
                    if (XRP_Charge.Substring(0, 1) == "-"){
                        Temp = Temp
                               + "[Down]"
                               + " %"
                               + XRP_Charge;
                    }
                    else if (XRP_Charge == "0.0" || XRP_Charge == "0")
                    {
                        Temp = Temp
                               + "[Static]"
                               + " %"
                               + XRP_Charge;
                    }
                    else
                    {
                        Temp = Temp
                        + "[UP]"
                        + " %"
                        + XRP_Charge;
                    }
                    return Temp;

                case "Dash":
                    Temp = Temp
                       + DASH
                       + " (USD) ";
                    if (DASH_Charge.Substring(0, 1) == "-"){
                        Temp = Temp
                               + "[Down]"
                               + " %"
                               + DASH_Charge;
                    }
                    else if (DASH_Charge == "0.0" || DASH_Charge == "0")
                    {
                        Temp = Temp
                               + "[Static]"
                               + " %"
                               + DASH_Charge;
                    }
                    else
                    {
                        Temp = Temp
                        + "[UP]"
                        + " %"
                        + DASH_Charge;
                    }
                    return Temp;
       
                default:
                    return "The command was not found.";
            }
        }
    }
}
