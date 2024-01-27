Imports System.ComponentModel
Imports System.IO
Imports System.Net

Public Class Download
    ' Percorso del file locale
    Private _percorsoFileLocale As String = "percorso_del_file\software.ver"

    <Browsable(True), Description("Percorso del file locale")>
    Public Property PercorsoFileLocale As String
        Get
            Return _percorsoFileLocale
        End Get
        Set(value As String)
            _percorsoFileLocale = value
        End Set
    End Property

    ' Percorso del file remoto
    Private _percorsoFileRemoto As String = "https://www.example.com/software/software.ver"

    <Browsable(True), Description("Percorso del file remoto")>
    Public Property PercorsoFileRemoto As String
        Get
            Return _percorsoFileRemoto
        End Get
        Set(value As String)
            _percorsoFileRemoto = value
        End Set
    End Property
    ' Funzione per cercare ricorsivamente il file nella root del software e nelle sue sotto-cartelle
    Function TrovaPercorsoFile(rootDirectory As String, nomeFile As String) As String
        Dim fileTrovato As String = String.Empty

        ' Cerca il file nella root
        Dim fileRoot As String = Path.Combine(rootDirectory, nomeFile)
        If File.Exists(fileRoot) Then
            Return fileRoot
        Else
            For Each directoryPath As String In Directory.GetDirectories(rootDirectory)
                Dim risultato As String = TrovaPercorsoFile(directoryPath, nomeFile)
                If Not String.IsNullOrEmpty(risultato) Then
                    Return risultato
                End If
            Next
        End If
        Return fileTrovato
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Leggi la versione del software dal file software.ver
        Dim versioneSoftware As String = File.ReadAllText(_percorsoFileRemoto)

        ' Leggi le informazioni dal file info.txt.ver
        Dim linee As String() = File.ReadAllLines(_percorsoFileLocale)

        ' Ciclo per creare controlli per ogni informazione
        For Each linea As String In linee
            ' Divide la riga utilizzando il separatore ||
            Dim parti As String() = linea.Split(New String() {"||"}, StringSplitOptions.RemoveEmptyEntries)

            ' Verifica se la riga contiene le informazioni desiderate
            If parti.Length = 3 Then
                Dim nomeProdotto As String = parti(0).Trim()
                Dim versioneProdotto As String = parti(1).Trim()
                Dim linkDownload As String = parti(2).Trim()

                ' Verifica se il prodotto corrisponde alla tua ricerca
                If nomeProdotto = "Nome del tuo software" Then
                    ' Estrai il nome del file dal link
                    Dim nomeFile As String = Path.GetFileName(New Uri(linkDownload).LocalPath)

                    ' Cerca il percorso del file
                    Dim percorsoFileDestinazione As String = TrovaPercorsoFile("percorso_del_tuo_software", nomeFile)

                    If Not String.IsNullOrEmpty(percorsoFileDestinazione) Then
                        ' Crea un'etichetta per visualizzare la versione del prodotto
                        Dim labelInfo As New Label()
                        labelInfo.Text &= "Versione: " & versioneProdotto & vbCrLf

                        ' Crea un pulsante per il download
                        Dim btnDownload As New Button()
                        btnDownload.Text = "Download"

                        ' Crea una TextBox per visualizzare la percentuale di download
                        Dim textBoxProgress As New TextBox()
                        textBoxProgress.ReadOnly = True
                        textBoxProgress.Text = "0%"

                        ' Aggiungi un gestore per l'evento Click del pulsante
                        AddHandler btnDownload.Click, Sub(senderBtn As Object, eBtn As EventArgs)
                                                          ' Crea un WebClient per il download
                                                          Dim webClient As New WebClient()

                                                          ' Aggiungi un gestore per l'evento DownloadProgressChanged
                                                          AddHandler webClient.DownloadProgressChanged, Sub(senderDownload As Object, eDownload As DownloadProgressChangedEventArgs)
                                                                                                            ' Aggiorna la TextBox con la percentuale di download
                                                                                                            textBoxProgress.Invoke(Sub() textBoxProgress.Text = eDownload.ProgressPercentage.ToString() & "%")
                                                                                                        End Sub

                                                          ' Avvia il download utilizzando il percorso del file trovato come destinazione
                                                          webClient.DownloadFileAsync(New Uri(linkDownload), percorsoFileDestinazione)

                                                          ' Disabilita il pulsante durante il download
                                                          btnDownload.Enabled = False
                                                      End Sub

                        ' Aggiungi label, pulsante e TextBox al form o al controllo contenitore
                        ' ...

                    Else
                        ' Il file non è stato trovato, gestisci di conseguenza
                        ' ...
                    End If
                End If
            End If
        Next
    End Sub
End Class
