Imports Windows.ApplicationModel.AppService
Imports Windows.ApplicationModel.Background
Public NotInheritable Class arbeiter
    Implements IBackgroundTask
    Dim connection As AppServiceConnection
    Dim serviceDeferral As BackgroundTaskDeferral
    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        serviceDeferral = taskInstance.GetDeferral()
        Dim details As AppServiceTriggerDetails = TryCast(taskInstance.TriggerDetails, AppServiceTriggerDetails)
        connection = details.AppServiceConnection
        AddHandler connection.RequestReceived, AddressOf OnRequestReceived
    End Sub
    Async Sub OnRequestReceived(sender As AppServiceConnection, args As AppServiceRequestReceivedEventArgs)
        Dim messageDeferral = args.GetDeferral()
        Try
            Dim input = args.Request.Message
            'Dim ink As InkStroke = CType(input("strich"), InkStroke)

            Debug.Write("received")

        Finally
            messageDeferral.Complete()
        End Try
    End Sub

End Class
