Imports Windows.ApplicationModel.AppService
Imports Windows.ApplicationModel.Background
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.UI
''' <summary>
''' Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
''' </summary>
NotInheritable Class App
    Inherits Application

    ''' <summary>
    ''' Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
    ''' werden verwendet, wenn die Anwendung zum Öffnen einer bestimmten Datei, zum Anzeigen
    ''' von Suchergebnissen usw. gestartet wird.
    ''' </summary>
    ''' <param name="e">Details über Startanforderung und -prozess.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' App-Initialisierung nicht wiederholen, wenn das Fenster bereits Inhalte enthält.
        ' Nur sicherstellen, dass das Fenster aktiv ist.

        If rootFrame Is Nothing Then
            ' Frame erstellen, der als Navigationskontext fungiert und zum Parameter der ersten Seite navigieren
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: Zustand von zuvor angehaltener Anwendung laden
            End If
            ' Den Frame im aktuellen Fenster platzieren
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
                ' und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
                ' übergeben werden
                rootFrame.Navigate(GetType(BlankPage1), e.Arguments)
            End If

            ' Sicherstellen, dass das aktuelle Fenster aktiv ist
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' Wird aufgerufen, wenn die Navigation auf eine bestimmte Seite fehlschlägt
    ''' </summary>
    ''' <param name="sender">Der Rahmen, bei dem die Navigation fehlgeschlagen ist</param>
    ''' <param name="e">Details über den Navigationsfehler</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
    ''' ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
    ''' unbeschädigt bleiben.
    ''' </summary>
    ''' <param name="sender">Die Quelle der Anhalteanforderung.</param>
    ''' <param name="e">Details zur Anhalteanforderung.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
        deferral.Complete()
    End Sub
    Protected Overrides Sub OnActivated(ByVal args As Windows.ApplicationModel.Activation.IActivatedEventArgs)

        If args.Kind = ActivationKind.Protocol Then
            Dim p As ProtocolActivatedEventArgs = TryCast(args, ProtocolActivatedEventArgs)
            Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)
            If rootFrame Is Nothing Then

                rootFrame = New Frame()
                Window.Current.Content = rootFrame
            End If
            'Dim daten = p.Data.First()
            'ApplicationData.Current.LocalSettings.Values("daten") = daten
            rootFrame.Navigate(GetType(DisplayInk), p)
            Window.Current.Activate()
        End If
    End Sub
    Dim _backgroundTaskDeferral As BackgroundTaskDeferral
    Dim _appServiceconnection As AppService.AppServiceConnection
    Protected Overrides Sub OnBackgroundActivated(args As BackgroundActivatedEventArgs)
        MyBase.OnBackgroundActivated(args)
        _backgroundTaskDeferral = args.TaskInstance.GetDeferral()
        AddHandler args.TaskInstance.Canceled, AddressOf OnTaskCanceled
        Dim details = TryCast(args.TaskInstance.TriggerDetails, AppServiceTriggerDetails)
        _appServiceconnection = details.AppServiceConnection
        AddHandler _appServiceconnection.RequestReceived, AddressOf OnRequestReceived
        AddHandler _appServiceconnection.ServiceClosed, AddressOf OnServiceClosed



    End Sub
    Private Sub OnTaskCanceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason)
        If (_backgroundTaskDeferral IsNot Nothing) Then
            _backgroundTaskDeferral.Complete()
            _backgroundTaskDeferral = Nothing
        End If
    End Sub
    Private Async Sub OnRequestReceived(sender As AppServiceConnection, args As AppServiceRequestReceivedEventArgs)


        Dim messageDeferral = args.GetDeferral()


        Dim daten = args.Request.Message
        Dim inkString = CType(daten("striche"), String)
        Dim inkBytes = Convert.FromBase64String(inkString)

        'TODO: wie komme  ins UI
        Dim ev As New UpdateUIEventArgs
        ev.InkBytes = inkBytes
        RaiseEvent OnUpdateUI(Me, ev)



        Dim result = New ValueSet()
        result.Add("ping", "")
        Await args.Request.SendResponseAsync(result)
        messageDeferral.Complete()
    End Sub
    Private Async Sub OnServiceClosed(sender As AppServiceConnection, args As AppServiceClosedEventArgs)
    End Sub
    Public Event OnUpdateUI(ByVal sender As Object, ByVal e As UpdateUIEventArgs)


End Class
Public Class UpdateUIEventArgs
    Inherits EventArgs
    Private _inkBytes As Byte()
    Public Property InkBytes() As Byte()
        Get
            Return _inkBytes
        End Get
        Set(ByVal value As Byte())
            _inkBytes = value
        End Set
    End Property
End Class
