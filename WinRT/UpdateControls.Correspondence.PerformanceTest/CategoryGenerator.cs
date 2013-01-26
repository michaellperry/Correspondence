using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Incentives.Model;
using UpdateControls.Correspondence;

namespace Incentives.ViewModel
{
    public class CategoryGenerator
    {
        private static readonly Regex Word = new Regex(@"\w+");

        private readonly Community _community;
        private readonly Company _company;
        private readonly Quarter _quarter;

        private List<Category> _categories;

        private static readonly string[] RawCategories =
        {
            "Account Management",
            "Business Development",
            "Certification/Recognition",
            "Direct Revenue",
            "Education/Coaching",
            "Industry Contribution/Leadership",
            "Industry Participation",
            "Networking",
            "Operations Support",
            "Recruiting",
            "Sales/Marketing Support",
            "Other"
        };

        private static readonly string[][] RawActivities =
        {
            new string[]
            {
                "Qualified Contact",
                "Consulting/Training/Placement Lead - Suspect",
                "Qualified Consulting Lead",
                "Consulting contract <=$10K",
                "Consulting contract >$10K and <=$100K",
                "Consulting contract >$100K",
                "Placement contract",
                "Training contract",
                "Participation in bluesheet creation"
            },
            new string[]
            {
                "Qualified contact",
                "Consulting - suspect",
                "Qualified consulting lead",
                "Consulting contract <=$10K",
                "Consulting contract >$10K and <=$100K",
                "Consulting contract >$100K",
                "Training suspect",
                "Training contract",
                "Placement suspect",
                "Qualified placement lead",
                "Placement contract"
            },
            new string[]
            {
                "SCM (agile)",
                "CSP (agile)",
                "Microsoft certification",
                "Microsoft MVP",
                "PMP (mgmt)"
            },
            new string[]
            {
                "Billing: 530 hrs/qtr",
                "Overtime: >530 <=650",
                "Required support - weekend",
                "Required support - 8pm-6am",
                "W2 work - zero pay"
            },
            new string[]
            {
                "Lead/coordinate ImprovingU track",
                "Present ImprovingU minicourse",
                "Personal coach (EGP)",
                "Client brown bag",
                "Adjunct instructor",
                "Project review presentation"
            },
            new string[]
            {
                "Publication - minor",
                "Publication - major",
                "Presentation - user group",
                "Presentation - conference",
                "Presentation - major",
                "Lead a user group",
                "Board participation",
                "Open source development"
            },
            new string[]
            {
                "User group attendance",
                "Professional group attendance",
                "Conference - weekend",
                "Conference - vacation day",
                "Conference - business day",
                "Relevant blog post",
                "Podcast contribution",
                "Podcast production"
            },
            new string[]
            {
                "Meeting",
                "Partner event"
            },
            new string[]
            {
                "Maintain business report",
                "Maintain KPY report",
                "Submit project inventory",
                "Project inventory update",
                "Weekend support",
                "Website development"
            },
            new string[]
            {
                "Interview - phone",
                "Interview - in person",
                "Referral - hire",
                "Referral - nonhire"
            },
            new string[]
            {
                "Proposal - large",
                "Proposal - small",
                "Support proposal (estimation)",
                "Support sales meeting",
                "Create news item",
                "Create webpage content",
                "Create produced collateral"
            },
            new string[]
            {
                "Other"
            }
        };

        private static string[][] RawRewards =
        {
            new string[]
            {
                "1/contact",
                "2/lead",
                "5/lead",
                "5/contract",
                "10/contract",
                "25/contract",
                "10/contract",
                "2/student",
                "2/sheet"
            },
            new string[]
            {
                "2/contact",
                "4/lead",
                "10/lead",
                "5/contract",
                "20/contract",
                "50/contract",
                "2/lead",
                "2/student",
                "2/lead",
                "5/lead",
                "25/contract"
            },
            new string[]
            {
                "10",
                "10",
                "10",
                "50",
                "10"
            },
            new string[]
            {
                "10/qtr",
                "1/hour",
                "1/hour",
                "1/hour",
                "2/hour"
            },
            new string[]
            {
                "10/qtr",
                "5/session",
                "3/session",
                "2/session",
                "2/day",
                "3/project"
            },
            new string[]
            {
                "10/publication",
                "20/publication",
                "10/presentation",
                "20/presentation",
                "40/presentation",
                "3/mtg",
                "5/qtr",
                "*"
            },
            new string[]
            {
                "1/mtg",
                "1/mtg",
                "3/day",
                "6/day",
                "0",
                "1/post",
                "2/podcast",
                "10/podcast"
            },
            new string[]
            {
                "1/mtg",
                "1/mtg"
            },
            new string[]
            {
                "5/report",
                "5/report",
                "2/entry",
                "1/update",
                "1/hour",
                "1/hour"
            },
            new string[]
            {
                "2",
                "4",
                "50",
                "5"
            },
            new string[]
            {
                "10/proposal",
                "5/proposal",
                "3/proposal",
                "4/mtg",
                "5/post",
                "2/page",
                "2/collateral"
            },
            new string[]
            {
                "*"
            }
        };

        public CategoryGenerator(Community community, Company company, Quarter quarter)
        {
            _community = community;
            _company = company;
            _quarter = quarter;
        }

        public async Task GenerateAsync()
        {
            _categories = new List<Category>();
            for (int categoryIndex = 0; categoryIndex < RawCategories.Length; ++categoryIndex)
            {
                string categoryDescription = RawCategories[categoryIndex];
                string categoryIdentifier = IdentifierOf(categoryDescription);

                var category = await _community.AddFactAsync(new Category(
                    _company,
                    categoryIdentifier));

                _categories.Add(category);
                category.Description = categoryDescription;
                category.Ordinal = categoryIndex;

                for (int activityIndex = 0; activityIndex < RawActivities[categoryIndex].Length; ++activityIndex)
                {
                    string activityDescription = RawActivities[categoryIndex][activityIndex];
                    string activityIdentifier = IdentifierOf(activityDescription);
                    string reward = RawRewards[categoryIndex][activityIndex];

                    var activity = await _community.AddFactAsync(new ActivityDefinition(
                        category,
                        activityIdentifier));
                    var activityReward = await _community.AddFactAsync(new ActivityReward(
                        activity,
                        _quarter));

                    activity.Description = activityDescription;
                    activity.Ordinal = activityIndex;

                    if (reward == "*")
                    {
                        activity.Qualifier = "point";
                        activityReward.Points = 1;
                    }
                    else
                    {
                        var parts = reward.Split('/');
                        activityReward.Points = int.Parse(parts[0]);
                        if (parts.Length == 2)
                        {
                            activity.Qualifier = parts[1];
                        }
                    }
                }
            }
        }

        public IEnumerable<Category> Categories
        {
            get { return _categories; }
        }

        private string IdentifierOf(string str)
        {
            var words = Word.Matches(str)
                .OfType<Match>()
                .Select(match => match.Value)
                .ToList();

            var segments = words.Take(1).Select(s => s.ToLower())
                .Concat(words.Skip(1).Select(s => InitialCaps(s)))
                .ToArray();
            return String.Join("", segments);
        }

        private string InitialCaps(string segment)
        {
            return segment.Substring(0, 1).ToUpper() + segment.Substring(1).ToLower();
        }
    }
}
