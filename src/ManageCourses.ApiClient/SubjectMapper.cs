using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class SubjectMapper
    {
        private static string[] ucasEnglish;
        private static string[] ucasMflMandarin;
        private static string[] ucasFurtherEducation;
        private static string[] ucasPrimary;
        private static string[] ucasIct;
        private static string[] ucasLanguageCat;
        private static string[] ucasOther;
        private static string[] ucasMathemtics;
        private static string[] ucasPhysics;
        private static string[] ucasScienceFields;
        private static string[] ucasMflMain;
        private static string[] ucasMflOther;
        private static string[] ucasMflWelsh;
        private static string[] ucasDesignAndTech;
        private static string[] ucasDirectTranslationSecondary;
        private static Dictionary<string,Regex> ucasNeedsMentionInTitle;
        private static string[] ucasUnexpected;

        private static IDictionary<string,string> ucasRename;

        static SubjectMapper()
        {
            ucasEnglish = new string[] 
            {                
                "english",
                "english language",
                "english literature"
            };

            ucasMflMandarin = new string[]
            {                
                "chinese",
                "mandarin"  
            };

            ucasMflMain = new string[] 
            {              
                "french",
                "german",
                "italian",
                "japanese",
                "latin",              
                "russian",
                "spanish"
            };

            ucasMflOther = new string[] 
            {            
                "arabic",
                "bengali",
                "gaelic",
                "greek",
                "hebrew",
                "urdu",
                "mandarin",
                "punjabi"
            };

            ucasMflWelsh = new string[] {
                "welsh"
            };

            ucasDesignAndTech = new string[]
            {
                "design and technology",
                "design and technology (food)",
                "design and technology (product design)",
                "design and technology (systems and control)",
                "design and technology (textiles)",
                "engineering"
            };

            ucasDirectTranslationSecondary = new string[]
            {
                "art / art & design",
                "business education",
                "citizenship",
                "classics",
                "communication and media studies",
                "computer studies",                
                "dance and performance",
                "drama and theatre studies",                
                "economics",
                "geography",
                "health and social care",                
                "history",
                "music",
                "outdoor activities",
                "physical education",
                "psychology",
                "religious education",
                "social science"
            };

            ucasNeedsMentionInTitle = new Dictionary<string, Regex>
            {
                {"humanities", new Regex("humanities")},
                {"science", new Regex("(?<!social |computer )science")},
                {"modern studies", new Regex("modern studies")}            
            };

            ucasFurtherEducation = new string[] 
            {
                "english as a second or other language",
                "further education",
                "higher education",
                "literacy",
                "numeracy",
                "post-compulsory"
            };

            ucasPrimary = new string[] 
            { 
                "early years",
                "upper primary",
                "primary",
                "lower primary"
            };

            ucasIct = new string[] {
                "information communication technology",
                "information technology"
            };

            ucasLanguageCat = new string[] 
            {
                "languages",
                "languages (african)",
                "languages (asian)",
                "languages (european)"
            };

            ucasOther = new string [] 
            {
                "leisure and tourism",
                "special educational needs"
            };

            ucasMathemtics = new string[] 
            {            
                "mathematics",
                "mathematics (abridged)"
            };

            ucasPhysics = new string[] 
            {
                "physics",
                "physics (abridged)"
            };            

            ucasScienceFields = new string[] 
            {
                "biology",
                "chemistry"
            };
                
            ucasUnexpected = new string[]
            {
                "construction and the built environment",
                "history of art",
                "home economics",
                "hospitality and catering",
                "personal and social education",
                "philosophy",
                "sport and leisure",
                "environmental science",
                "law"
            };

            ucasRename = new Dictionary<string,string>()
            {
                {"chinese", "mandarin"},
                {"art / art & design", "art and design"},
                {"business education", "business studies"},
                {"computer studies", "computing"},
                {"science", "balanced science"},
                {"dance and performance", "dance"},
                {"drama and theatre studies", "drama"},
                {"social science", "social sciences"}
            };
        }
        public IEnumerable<string> GetSubjectList(string courseTitle, IEnumerable<string> ucasSubjects)
        {            
            ucasSubjects = ucasSubjects.Select(x => x.ToLowerInvariant().Trim());
            courseTitle = courseTitle.ToLowerInvariant().Trim();

            // if unexpected throw.
            if (ucasSubjects.Intersect(ucasUnexpected).Any())
            {
                throw new ArgumentException("found unsupported subject name");
            }            
            
            // Primary?
            else if (ucasSubjects.Intersect(ucasPrimary).Any())
            {
                return MapToPrimarySubjects(ucasSubjects);
            }
            
            // further education?
            else if (ucasSubjects.Intersect(ucasFurtherEducation).Any())
            {
                return new List<string>() { "Further education" };                
            }

            // now we're in secondary world
            else
            {
                return MapToSecondarySubjects(courseTitle, ucasSubjects);
            }
        }

        private IEnumerable<string> MapToPrimarySubjects(IEnumerable<string> ucasSubjects)
        {
            var primarySubjects = new List<string>() { "Primary" };

            var ucasPrimaryLanguageSpecialisation = new string[] {}            
                .Concat(ucasLanguageCat)
                .Concat(ucasMflMain)
                .Concat(ucasMflOther);

            var ucasPrimaryScienceSpecialisation = new string[] {"science"}            
                .Concat(ucasMathemtics)
                .Concat(ucasPhysics)
                .Concat(ucasScienceFields);

            if(ucasSubjects.Intersect(ucasEnglish).Any())
            {
                primarySubjects.Add("Primary with English");
            }

            if(ucasSubjects.Contains("geography"))
            {
                primarySubjects.Add("Primary with geography");
            }

            if(ucasSubjects.Contains("history"))
            {
                primarySubjects.Add("Primary with history");
            }

            if(ucasSubjects.Intersect(ucasMathemtics).Any())
            {
                primarySubjects.Add("Primary with mathematics");
            }

            if(ucasSubjects.Intersect(ucasPrimaryLanguageSpecialisation).Any())
            {
                primarySubjects.Add("Primary with modern languages");
            }            

            if(ucasSubjects.Contains("physical education"))
            {
                primarySubjects.Add("Primary with physical education");
            }

            if(ucasSubjects.Intersect(ucasPrimaryScienceSpecialisation).Any())
            {
                primarySubjects.Add("Primary with science");
            }

            return primarySubjects;
        }

        public IEnumerable<string> MapToSecondarySubjects(string courseTitle, IEnumerable<string> ucasSubjects)
        {
            var secondarySubjects = new List<string>();

            //  maths
            if (ucasSubjects.Intersect(ucasMathemtics).Any())
            {
                secondarySubjects.Add("Mathematics");
            }

            //  physics
            if (ucasSubjects.Intersect(ucasPhysics).Any())
            {
                secondarySubjects.Add("Physics");
            }

            //  ict
            if (ucasSubjects.Intersect(ucasIct).Any())
            {
                secondarySubjects.Add("Information and communication technology (ICT)");
            }

            //  dt
            if (ucasSubjects.Intersect(ucasDesignAndTech).Any())
            {
                secondarySubjects.Add("Design and technology");
            }

            //  mfl other
            if (ucasSubjects.Intersect(ucasLanguageCat).Any() && !ucasSubjects.Intersect(ucasMflMandarin).Any() && !ucasSubjects.Intersect(ucasMflMain).Any())
            {
                secondarySubjects.Add("Modern language (other)");
            }

            // mfl - mandarin chinese
            if (ucasSubjects.Intersect(ucasMflMandarin).Any())
            {
                secondarySubjects.Add("Mandarin");
            }

            //  mfl
            foreach(var ucasSubject in ucasSubjects.Intersect(ucasMflMain))
            {
                secondarySubjects.Add(MapToSubjectName(ucasSubject));
            }

            //  sciences
            foreach(var ucasSubject in ucasSubjects.Intersect(ucasScienceFields))
            {
                secondarySubjects.Add(MapToSubjectName(ucasSubject));
            }

            //  direct translation
            foreach(var ucasSubject in ucasSubjects.Intersect(ucasDirectTranslationSecondary))
            {
                secondarySubjects.Add(MapToSubjectName(ucasSubject));
            }

            //  needs mention
            foreach(var ucasSubject in ucasSubjects.Intersect(ucasNeedsMentionInTitle.Keys))
            {
                if (ucasNeedsMentionInTitle[ucasSubject].IsMatch(courseTitle))
                {
                    secondarySubjects.Add(MapToSubjectName(ucasSubject));
                }
            }

            //  english (check title if not only one ambiguous)
            if (ucasSubjects.Intersect(ucasEnglish).Any())
            {
                if (!secondarySubjects.Any() || courseTitle.IndexOf("english") > -1)
                {
                    secondarySubjects.Add("English");
                }
            }

            // if nothing else yet, try welsh
            if (!secondarySubjects.Any() && ucasSubjects.Intersect(ucasMflWelsh).Any())
            {
                secondarySubjects.Add("Welsh");
            }

            return secondarySubjects;
        }

        private string MapToSubjectName(string ucasSubject)
        {
            // rename if desired
            var res = ucasRename.TryGetValue(ucasSubject, out string mappedSubject) ? mappedSubject : ucasSubject;
            
            // capitalise
            res = res.Substring(0,1).ToUpper() + res.Substring(1).ToLower();
            
            // ensure English is always correctly cased
            return res.Replace("english", "English");           
        }
    }
}
