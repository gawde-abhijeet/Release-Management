using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TFS_POC.Model;
using TFS_POC.Model.GitCommits;

namespace TFS_POC
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO - Read the default Project Colection from Configurations
            string defProjectCol = "TFS2013TPC03";
            string defProjectName = "SoltoDealEvaluation_4592Git";

            ReadThroughProjectCollectionWithAssociatedProjects();
            Console.WriteLine("-----------------------------------------------------------");
            ReadThroughWorkItemsForSpecificProjectCollection(defProjectCol);
            Console.WriteLine("-----------------------------------------------------------");
            //TODO - Display Project Specific Details

            ReadThroughGitReposForSpecifcProject(defProjectCol);

            Console.ReadLine();
        }

        private static async void ReadThroughGitReposForSpecifcProject(string projectCol)
        {
            Uri uri = getTfsUri(projectCol);
            string repoName = "IdcTraining"; //TODO - Read from Config

            HttpClientHandler handler = new HttpClientHandler();
            handler.Credentials = getCredentials(uri);

            using (var client = new HttpClient(handler))
            {
                // Get Git repositories
                var url = string.Format("{0}/_apis/git/repositories?api-version=1.0", uri);

                var response = await Get(client, url);
                var repositories = JsonConvert.DeserializeObject<Repository>(response);

                foreach (var r in repositories.Value)
                {
                    Console.WriteLine(r.name);
                }

                Console.WriteLine("------- Git Commits: IdcTraining -------");

                var repourl = string.Format("{0}/_apis/git/SoltoDealEvaluation_4592Git/repositories/{1}/commits?api-version=1.0", uri, repoName);

                var resp = await Get(client, repourl);

                var gitCommits = JsonConvert.DeserializeObject<GitCommits>(resp);

                foreach (var commit in gitCommits.Value)
                {
                    Console.WriteLine("     {0}", commit.comment);
                }
            }
        }

        private static async Task<string> Get(HttpClient client, string url)
        {
            var result = string.Empty;

            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }

        private static void ReadThroughWorkItemsForSpecificProjectCollection(string projectCol)
        {
            // Connect to the work item store
            TfsTeamProjectCollection tfsProjectCol = new TfsTeamProjectCollection(getTfsUri(projectCol));

            WorkItemStore workItemStore = (WorkItemStore)tfsProjectCol.GetService(typeof(WorkItemStore));

            // Run a query.
            WorkItemCollection queryResults = workItemStore.Query(
               "Select [ID], [State], [Title], [Assigned To], [Changed Date] " +
               "From WorkItems " +
               "Where [Work Item Type] = 'Task' " +
                //"AND [State] = 'Active' " + 
                "AND ([Assigned to] = @Me " + 
                "OR [Closed by] = @Me) " +
               "Order By [State] Asc, [Changed Date] Desc");

            foreach (WorkItem item in queryResults)
            {
                Console.WriteLine(string.Format("{0} | {1} | {2} | {3}",
                            item.Fields["ID"].Value,
                            item.Fields["Changed Date"].Value,
                            item.Fields["Assigned To"].Value,
                            item.Fields["Title"].Value
                        ));
            }
        }

        private static void ReadThroughProjectCollectionWithAssociatedProjects()
        {
            Uri tfsUri = getTfsUri();

            TfsConfigurationServer configurationServer =
                TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);

            // Get the catalog of team project collections
            ReadOnlyCollection<CatalogNode> collectionNodes = configurationServer.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection },
                false, CatalogQueryOptions.None);

            // List the team project collections
            foreach (CatalogNode collectionNode in collectionNodes)
            {
                // Use the InstanceId property to get the team project collection
                Guid collectionId = new Guid(collectionNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection teamProjectCollection = configurationServer.GetTeamProjectCollection(collectionId);

                // Print the name of the team project collection
                Console.WriteLine("Collection: " + teamProjectCollection.Name);

                // Get a catalog of team projects for the collection
                ReadOnlyCollection<CatalogNode> projectNodes = collectionNode.QueryChildren(
                    new[] { CatalogResourceTypes.TeamProject },
                    false, CatalogQueryOptions.None);

                // List the team projects in the collection
                foreach (CatalogNode projectNode in projectNodes)
                {
                    Console.WriteLine(" Team Project: " + projectNode.Resource.DisplayName);
                }
            }
        }

        private static Uri getTfsUri(string projectCol = "")
        {
            // TODO - Read the default TFS Server URL from the configuration
            string defTfsUri = "https://tfs2013.accenture.com/tfs";

            if (!string.IsNullOrEmpty(projectCol))
                defTfsUri = string.Format("{0}/{1}", defTfsUri, projectCol);
                   
            return new Uri(defTfsUri);
        }

        private static CredentialCache getCredentials(Uri uri)
        {
            var credentialCache = new CredentialCache();

            credentialCache.Add(new Uri(uri.GetLeftPart(UriPartial.Authority)),
                "NTLM",
                new NetworkCredential("abhijeet.gawde", "Apes#2015Feb$", "DIR")
            );
            return credentialCache;
        }
    }
}
