using asplib.Remoting;
using iie;
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

                // --- Begin test button ---
                .Test(async (el, ev) =>
                {
                    var result = await TestServer.Test("minimaltest.websharper.spa");
                    testSummary.Clear();
                    testSummary.AppendMany(result.Summary);
                    if (!result.Passed)
                    {
                        JS.Window.Location.Assign(TestResult.Path);
                    }
                })
                .TestSummaryContainer(
                    testSummary.View.DocSeqCached((string x) =>
                        new Template.Index.TestSummaryItem().Line(x).Doc()
                    )
                )
                // --- End test button ---

                .Doc();
        }

        /// <summary>
        /// Subpage: The content is built up entirely on the client and sent to
        /// the server in a JSON-Compatible data type.
        /// </summary>
        /// <returns></returns>
        public static WebSharper.UI.Doc WithStaticDoc()
        {
            var contentModel = new ListModel<string, string>(s => s);
            var contentTextBox = Var.Create("");

            return new Template.Withstatic()
                .ContentTextBox(contentTextBox)
                .Back((el, ev) =>
                {
                    IndexDoc().RunById("page");
                })
                .ListContainer(
                    contentModel.View.DocSeqCached((string x) =>
                        new Template.Withstatic.ListItem().Item(x).Doc()
                    )
                )
                .Submit(async (el, ev) =>
                {
                    contentModel.Add(contentTextBox.Value);
                    contentTextBox.Set("");
                    var json = new List<string>(contentModel.Value);
                    await StaticRemoting.Put(json);
                })
                .Doc();
        }

        /// <summary>
        /// Subpage: The content is built up on the server side, session
        /// persistence is handled either on the client (ViewState) or on the
        /// server (Session/Database)
        /// </summary>
        /// <returns></returns>
        public static WebSharper.UI.Doc WithStorageDoc()
        {
            var varStorage = Var.Create(asplib.Model.Storage.ViewState);
            var varViewState = Var.Create("");
            var contentModel = new ListModel<string, string>(s => s);
            var contentTextBox = Var.Create("");

            return new Template.Withstorage()
                .ViewState(varViewState)
                .ContentTextBox(contentTextBox)

                .Back((el, ev) =>
                {
                    IndexDoc().RunById("page");
                })

                // <input ws-var="Storage" value="${ViewState}" type="radio" /> ViewState
                // yields:
                // Using ws-var on a <input type="radio"> node is not supported yet
                // Thus programmatically:
                .StorageRadio(
                    label(
                        radio(varStorage, asplib.Model.Storage.ViewState),
                        "ViewState"
                     ),
                    label(
                        radio(varStorage, asplib.Model.Storage.Session),
                        "Session"
                     ),
                    label(
                        radio(varStorage, asplib.Model.Storage.Database),
                        "Database"
                     )
                )
                .ListContainer(
                    contentModel.View.DocSeqCached((string x) =>
                        new Template.Withstatic.ListItem().Item(x).Doc()
                    )
                )

                .ChangeStorage(async (el, ev) =>
                {
                    // Set the static storage override globally for the
                    // instance, as the concept of SessionStorage makes little
                    // sense, when it's required to carry it's value along with
                    // the single stored Storage object itself.
                    await StorageServer.SetStorage(varStorage.Value);

                    // Reload the object from the other storage.
                    var newContent = await StorageRemoting.Reload(varViewState.Value);
                    varViewState.Value = newContent.ViewState;
                    contentModel.Set(newContent.Content);
                })
                .Submit(async (el, ev) =>
                {
                    // Retrieve the old stateful object, perform the transition
                    // on it with all new input data passed as method arguments
                    // in one single step, and immediately save the object in
                    // its new state, thus making it effectively immutable from
                    // here on.
                    var newContent = await StorageRemoting.Add(varViewState.Value, contentTextBox.Value);
                    varViewState.Value = newContent.ViewState;

                    // Update the view according to the model.
                    contentModel.Set(newContent.Content);

                    // Clear entirely on the client side.
                    contentTextBox.Set("");
                })

                .Doc();
        }
    }
}