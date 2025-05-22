using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalliAPI.BusinessLogic;

namespace CalliAPI.Utilities
{

    /// <summary>
    /// Centralized definitions for tooltips used across the application.
    /// </summary>
    internal class TooltipDefinitions
    {


        public static readonly Dictionary<string, string> Keywords = new()
        {
        { "open", "Open means the matter has a status of 'Open' in Clio." },
        { "unworked", "Unworked means the matter has no open tasks and is in an early-stage workflow." },
        { "prefile", ClioService.GetPrefileStageDescription() },
        { "fastfetch", "FastFetch retrieves up to 10,000 matters in parallel since a given date." },
        { "713", "Refers to practice areas ending in 7 or 13, typically Chapter 7 or 13 bankruptcy." },
        { "practice area" , "The practice area is the category of law under which the matter falls." },
        { "matter", "A matter is a case or project that a law firm is working on." },
        { "task", "A task is an action item or assignment related to a matter." },
        { "status", "The status field in Clio indicates the current state of a matter or task." },
        { "complete", "Complete means the task has been finished." },
        { "filter", "Filter allows you to narrow down the list of matters based on criteria." }
        };


    }
}
