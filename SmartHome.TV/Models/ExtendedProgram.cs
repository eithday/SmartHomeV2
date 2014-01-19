using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvDatabase;

namespace SmartHome.TV.Models
{
  public class ExtendedProgram : Program
  {
    public ExtendedProgram(int idProgram, int idChannel, DateTime startTime, DateTime endTime, string title, string description, string genre, Program.ProgramState state, DateTime originalAirDate, string seriesNum, string episodeNum, string episodeName, string episodePart, int starRating, string classification, int parentalRating)
      : base(idProgram, idChannel, startTime, endTime, title, description, genre, state, originalAirDate, seriesNum, episodeNum, episodeName, episodePart, starRating, classification, parentalRating)
    { }
    public int ProgramLength { get; set; }
    public string ChannelName { get; set; }
    public string ChannelImage { get; set; }
    public List<Program> Programs { get; set; }

  }
}