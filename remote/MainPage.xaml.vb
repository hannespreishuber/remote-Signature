' Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

Imports Windows.Devices.Bluetooth.Advertisement
Imports Windows.Devices.Sensors
Imports Windows.Storage.Streams
Imports Windows.UI.Core
''' <summary>
''' Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Private _sensor As Accelerometer

    Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        '        Dim publisher = New BluetoothLEAdvertisementPublisher()


        _sensor = Accelerometer.GetDefault(AccelerometerReadingType.Linear)


        ' AddHandler _sensor.Shaken, AddressOf geschuettelt   leider nicht supported
        AddHandler _sensor.ReadingChanged, AddressOf SensorLiest


        'Dim manufacturerData = New BluetoothLEManufacturerData()
        'manufacturerData.CompanyId = 1


        'Dim writer = New DataWriter()
        'writer.WriteString("ppedv kaufen")

        'manufacturerData.Data = writer.DetachBuffer()

        'publisher.Advertisement.ManufacturerData.Add(manufacturerData)
        'publisher.Start()
    End Sub
    Private Async Sub geschuettelt(sender As Object, e As AccelerometerShakenEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     txtOw.Text = e.Timestamp.ToLocalTime.ToString
                                                                 End Sub)
    End Sub

    Private Async Sub SensorLiest(sender As Object, args As AccelerometerReadingChangedEventArgs)


        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     If args.Reading.AccelerationX > 1 Then
                                                                         txtOw.Text = "shake"

                                                                     End If

                                                                 End Sub)
    End Sub
End Class
