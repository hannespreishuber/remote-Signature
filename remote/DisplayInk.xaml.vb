' Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

Imports Windows.Storage.Streams
Imports Windows.UI
Imports Windows.UI.Core
''' <summary>
''' Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
''' </summary>
Public NotInheritable Class DisplayInk
    Inherits Page
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim view = ApplicationView.GetForCurrentView()

        view.TitleBar.BackgroundColor = Colors.DarkGreen
        view.TitleBar.ForegroundColor = Colors.White
        AddHandler CType(App.Current, App).OnUpdateUI, AddressOf UpdateUI


    End Sub

    Public Async Sub UpdateUI(sender As Object, args As UpdateUIEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Async Sub()
                                                                     Dim img = New BitmapImage()
                                                                     Using ms = New InMemoryRandomAccessStream()
                                                                         Await ms.WriteAsync(args.InkBytes.AsBuffer())
                                                                         ms.Seek(0)
                                                                         img.SetSource(ms)
                                                                     End Using
                                                                     InkCanvas1.Source = img
                                                                 End Sub)




    End Sub
End Class
