using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DR.Common.RESTClient;

namespace HelloBot
{
    public static class MUClient
    {
        private static readonly IJsonClient JsonClient = new JsonClient(true) { BaseURL = "https://www.dr.dk/mu-online/api/1.3/" };

        public static IEnumerable<NowNext> GetNowNextForAllActiveDRChannels()
        {
            return JsonClient.Get<IEnumerable<NowNext>>("/schedule/nownext-for-all-active-dr-tv-channels/");
        }
        public class NowNext
        {
            public string ChannelSlug { get; set; }
            public ScheduleBroadcast Now { get; set; }
            public IEnumerable<ScheduleBroadcast> Next { get; set; }
        }

        public class ScheduleBroadcast
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public ProgramCardListItem ProgramCard { get; set; }
            public bool ProgramCardHasPrimaryAsset { get; set; }
            public bool SeriesHasProgramCardWithPrimaryAsset { get; set; }
        }
        public class ProgramCardListItem
        {
            public string Slug { get; set; }
            public string Title { get; set; }
            public string PrimaryImageUri { get; set; }
            public string SeriesTitle { get; set; }
            public string SeriesSlug { get; set; }
        }
        
    }
}
 