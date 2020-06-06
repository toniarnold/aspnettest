using asplib.Remoting;
using iselenium;
using System.Collections.Generic;
using WebSharper;
using WebSharper.JavaScript;
using WebSharper.UI;
using WebSharper.UI.Client;
using static WebSharper.UI.Client.Html;

namespace minimal.websharper.spa
{
    [JavaScript]
    public class App
    {
        [SPAEntryPoint]
        public static void ClientMain()
        {
            IndexDoc().RunById("page");
        }

        /// <summary>
        /// Main page as template.
        /// </summary>
        /// <returns></returns>
        public static WebSharper.UI.Doc IndexDoc()
        {
            var testSummary = new ListModel<string, string>(s => s);

            return new Template.Index.Main()

                // Links to sub-pages
                .WithStatic((el, ev) =>
                {
                    WithStaticDoc().RunById("page");
                })
                .WithStorage((el, ev) =>
                {
                    WithStorageDoc().RunById("page");
                })

                // Test button with result summary
                .Test(async (el, ev) =>
                {
                    var result = await RemoteTestRunner.Run("minimaltest.websharper.spa");
                    testSummary.Clear();
                    testSummary.AppendMany(result.Summary);
                    if (!result.Passed)
                    {
                        JS.Window.Location.Assign(TestResultSite.PathFailed);
                    }
                })
                .TestSummaryContainer(
                    testSummary.View.DocSeqCached((string x) =>
                        new Template.Index.TestSummaryItem().Line(x).Doc()
                    )
                )

                .Doc();
        }

        /// <summary>
        /// Subpage: The content is built up entirely on the client and sent to
        /// the server in a JSON-Compatible data type.
        /// </summary>
        /// <returns></returns>
        public static WebSharper.UI.Doc WithStaticDoc()
        {
            var contentList = new ListModel<string, string>(s => s);
            var contentTextBox = Var.Create("");

            return new Template.Withstatic()
                .ContentTextBox(contentTextBox)
                .Back((el, ev) =>
                {
                    IndexDoc().RunById("page");
                })
                .ListContainer(
                    contentList.View.DocSeqCached((string x) =>
                        new Template.Withstatic.ListItem().Item(x).Doc()
                    )
                )
                .Submit(async (el, ev) =>
                {
                    contentList.Add(contentTextBox.Value);
                    contentTextBox.Set("");
                    var conent = new List<string>(contentList.Value);
                    await StaticRemoting.Put(conent);
                })
                .Doc();
        }

        /// <summary>
        /// Subpage: The content is built up on the server side, session
        /// persistence is handled either on the client (ViewState) or on the
        /// server (Session/Database).
        /// </summary>
        /// <returns></returns>
        public static WebSharper.UI.Doc WithStorageDoc()
        {
            var storage = Var.Create(asplib.Model.Storage.ViewState);
            var contentList = new ListModel<string, string>(s => s);
            var contentTextBox = Var.Create("");

            // Setup the reactive ViewState storage.
            var viewState = Var.Create("");
            return WebSharper.UI.Doc.ConcatMixed(
                input(viewState, attr.type("hidden"))
                ,
                new Template.Withstorage()
                    .ContentTextBox(contentTextBox)

                    .Back((el, ev) =>
                    {
                        IndexDoc().RunById("page");
                    })

                    // <input ws-var="Storage" value="${ViewState}" type="radio" /> ViewState
                    // in the template yields:
                    // Using ws-var on a <input type="radio"> node is not supported yet, thus programmatic.
                    // Auto-generates the name: <input name="uinref4" type="radio">, manual ids for test.SelectID().
                    .StorageRadio(
                        label(
                            radio(storage, asplib.Model.Storage.ViewState, attr.id("storageViewState")),
                            "ViewState"
                         ),
                        label(
                            radio(storage, asplib.Model.Storage.Session, attr.id("storageSession")),
                            "Session"
                         ),
                        label(
                            radio(storage, asplib.Model.Storage.Database, attr.id("storageDatabase")),
                            "Database"
                         )
                    )
                    .ListContainer(
                        contentList.View.DocSeqCached((string x) =>
                            new Template.Withstatic.ListItem().Item(x).Doc()
                        )
                    )

                    .ChangeStorage(async (el, ev) =>
                    {
                        // Set the static storage override globally for the
                        // instance, as the concept of SessionStorage makes little
                        // sense when it's required to carry it's value along with
                        // the single stored Storage object itself.
                        await StorageServer.SetStorage(storage.Value);

                        // Reload the object from the other storage.
                        var newContent = await StorageRemoting.Reload(viewState.Value);

                        // Write back the changed model object state
                        // to the reactive variable.
                        viewState.Value = newContent.ViewState;

                        // Update the view according to the model.
                        contentList.Set(newContent.Content);
                    })
                    .Submit(async (el, ev) =>
                    {
                        // Retrieve the old stateful object, perform the transition
                        // on it with all new input data passed as method arguments
                        // in one single step, and immediately save the object in
                        // its new state, thus making it effectively immutable from
                        // here on.
                        var newContent = await StorageRemoting.Add(viewState.Value, contentTextBox.Value);

                        // Write back the changed model object state
                        // to the reactive variable.
                        viewState.Value = newContent.ViewState;

                        // Update the view according to the model.
                        contentList.Set(newContent.Content);

                        // Clear entirely on the client side.
                        contentTextBox.Set("");
                    })

                    .Doc()
            );
        }
    }
}