' Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

Imports Windows.ApplicationModel.AppService
Imports Windows.Storage.Streams
Imports Windows.System
Imports Windows.System.RemoteSystems
Imports Windows.UI.Core
Imports Windows.UI.Input.Inking
''' <summary>
''' Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
''' </summary>
Public NotInheritable Class BlankPage1
    Inherits Page
    Dim rm As RemoteSystemWatcher
    Private devices = New ObservableCollection(Of RemoteSystem)


    Private Sub BlankPage1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        ink1.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Touch
        AddHandler ink1.InkPresenter.StrokesCollected, AddressOf neuStroke
        ' canvas1.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Touch
    End Sub

    Private Async Sub neuStroke(sender As Object, e As InkStrokesCollectedEventArgs)
        Dim ink As InkStroke = e.Strokes.Last.Clone
        ' canvas1.InkPresenter.StrokeContainer.AddStroke(ink)
        'dim stream as imemorystream
        'Await ink1.InkPresenter.StrokeContainer.SaveAsync(Stream)
        'Dim sr = Stream.AsStreamForRead
        'Dim b(sr.Length) As Byte
        'sr.Read(b, 0, sr.Length)
        'Stream.Dispose()
        Dim inkbytes() As Byte
        Using stream As InMemoryRandomAccessStream = New InMemoryRandomAccessStream()

            Await ink1.InkPresenter.StrokeContainer.SaveAsync(stream)

            stream.Seek(0)
            Dim bmp As New BitmapImage
            bmp.SetSource(stream)
            canvas1.Source = bmp

            stream.Seek(0)
            Dim ms As MemoryStream = New MemoryStream()
            stream.AsStream.CopyTo(ms)
            inkbytes = ms.ToArray()
        End Using


        'remotecall

        Dim con = New RemoteSystemConnectionRequest(selectedRS)

        Using connection = New AppServiceConnection()
            connection.AppServiceName = "de.ppedv"
            connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
            Dim status As AppServiceConnectionStatus = Await connection.OpenRemoteAsync(con)


            Dim inputs = New ValueSet()
            inputs.Add("striche", Convert.ToBase64String(inkbytes))
            Dim response As AppServiceResponse = Await connection.SendMessageAsync(inputs)
            If response.Status = AppServiceResponseStatus.Success AndAlso response.Message.ContainsKey("result") Then
                Dim resultText = response.Message("result").ToString()
                If Not String.IsNullOrEmpty(resultText) Then
                    '    Result.Text = resultText
                Else
                End If

                Return
            End If
        End Using
    End Sub

    Private Async Sub BuildDeviceList()

        Dim status = Await RemoteSystem.RequestAccessAsync()
        If (status = RemoteSystemAccessStatus.Allowed) Then
            rm = RemoteSystem.CreateWatcher()
            AddHandler rm.RemoteSystemAdded, AddressOf RmAdded
            rm.Start()
        End If
    End Sub


    Private Async Sub RmAdded(sender As Object, args As RemoteSystemAddedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     devices.Add(args.RemoteSystem)

                                                                 End Sub)
    End Sub
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        BuildDeviceList()

    End Sub

    Private Sub GridView_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    End Sub
    Dim selectedRS As RemoteSystem
    Private Async Sub GridView_ItemClick(sender As Object, e As ItemClickEventArgs)
        selectedRS = e.ClickedItem
        Dim launchUriStatus = Await RemoteLauncher.LaunchUriAsync(New RemoteSystemConnectionRequest(selectedRS),
            New Uri("notes:"))



    End Sub
End Class
