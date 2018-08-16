' DATA: 15/08/2018 09:38h
' Backup automático de dados
' Aplicativo criado para realizar o Backup Automático de dados 
' do sistema da Diretoria de Saúde
' Desenvolvedor: Leonardo Metelys - ALEAM
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net.Mail.SmtpClient
Imports System.Net.Mime
Imports System.Data.Odbc
Imports System.Data
Imports System.Data.OleDb
Imports System.Net.Mail
Imports System.Net

Public Class Form1

    Public sDataAtual As String = Today
    Public HoraExecucao1 As String = "07:00"
    Public HoraExecucao2 As String = "14:30"
    Public HoraExecucao3 As String = "09:30"
    Public HoraExecucao4 As String = "00:00"
    Public sMudaDataDia As Boolean = False
    Dim CaminhoCopiaemUso As String = ""

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        End
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        sMudaDataDia = False

        'Inicializar o Timer
        Timer2.Enabled = True

        Dim sData As String = Mid(Now, 1, 10)
        TxtDataInicial.Text = "01/" + Mid(sData, 4, 7)
        TxtDataFinal.Text = "01/" + Mid(sData, 4, 7)

        TxtDataInicial.Enabled = False

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        'Colocar na bandeja de tarefas
        If Me.WindowState = FormWindowState.Normal Then

            'NotifyIcon1.Visible = False
            'Me.WindowState = FormWindowState.Minimized
            Me.Hide()

        End If

        'Habilitar a Atualização
        Timer1.Enabled = True

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick


        Dim sDataHoje As String = ""
        Dim sDataHoraHoje As DateTime = Now
        Dim sHora As String = Mid(sDataHoraHoje, 12, 5)  'Pegar a Hora Atual
        Dim sData As String = Mid(sDataHoraHoje, 1, 10)

        sDataHoje = sDataAtual

        'Executar a Atualização
        'Me.WindowState = FormWindowState.Normal
        'Me.Visible = True


        'A cada 30segudos checar se atingiu a data e a hora determinadas para as três execuções diárias
        If sDataAtual = sData And sHora = HoraExecucao1 Then

            'Atualização iniciada
            Timer1.Enabled = False

            Timer2.Enabled = False

            'Rodar a atualização
            Timer3.Enabled = True

        End If

        'Backup da tarde
        If sDataAtual = sData And sHora = HoraExecucao2 Then

            'Atualização iniciada
            Timer1.Enabled = False

            Timer2.Enabled = False

            'Rodar a atualização
            Timer3.Enabled = True

        End If


        'Se é meia-noite é preciso alterar a data para a data do novo dia 
        If sDataAtual <> sData And sHora = HoraExecucao4 Then

            'Deve entrar aqui somente uma vez à meia-noite, porque após a variável sDataAtual ser atualizada
            'nucam será diferente de sData
            sMudaDataDia = True

        End If

        If sMudaDataDia = True Then
            'Atualizar a data Atual
            sDataAtual = Today

        End If

    End Sub

    Sub copiarDiretorio(ByVal CaminhoFonte As String, ByVal CaminhoDestino As String, Optional ByVal Sobrepor As Boolean = False)

        Dim DiretorioFonte As DirectoryInfo = New DirectoryInfo(CaminhoFonte)
        Dim DiretorioDestino As DirectoryInfo = New DirectoryInfo(CaminhoDestino)


        If DiretorioFonte.Exists Then

            If Not DiretorioDestino.Parent.Exists Then
                LbRegistros.Text = " O diretório de destino não existe : " + DiretorioDestino.FullName
                LbRegistros.Visible = True
            End If

            Application.DoEvents()

            'copia todos os arquivos do diretório
            Dim arquivos As FileInfo

            For Each arquivos In DiretorioFonte.GetFiles()

                If Sobrepor Then
                    arquivos.CopyTo(Path.Combine(DiretorioDestino.FullName, arquivos.Name), True)
                Else

                    If Not File.Exists(Path.Combine(DiretorioDestino.FullName, arquivos.Name)) Then

                        arquivos.CopyTo(Path.Combine(DiretorioDestino.FullName, arquivos.Name), False)

                        Label5.Text = " Arquivo " + arquivos.Name + " copiando... "
                        Label5.Visible = True
                        Application.DoEvents()


                    End If

                End If

            Next

            'copia todos os subdiretorios usando recursao
            Dim subdir As DirectoryInfo
            For Each subdir In DiretorioFonte.GetDirectories()

                'Criar pasta em diretório
                System.IO.Directory.CreateDirectory(CaminhoCopiaemUso + "\" + subdir.Name)
                copiarDiretorio(subdir.FullName, Path.Combine(DiretorioDestino.FullName, subdir.Name), False)

                Label5.Text = " Diretório " + subdir.Name + " copiando... "
                Label5.Visible = True
                Application.DoEvents()

            Next

        Else

            LbRegistros.Text = " Diretório origem não existe : " + DiretorioFonte.FullName
            LbRegistros.Visible = True

            'Throw New DirectoryNotFoundException("Diretório origem não existe " + DiretorioFonte.FullName)

        End If

    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick

        'Desabilitar o Timer
        Timer3.Enabled = False

        Me.Show()

        Application.DoEvents()

        Dim sData As String = Mid(Now, 1, 10)
        Dim sHora As String = Mid(Now, 12, 5)  'Pegar a Hora Atual
        Dim sHoraFile = Replace(sHora, ":", "_")
        Dim CaminhoCopia = "C:\Backup\" + "SYSCOM_" + Mid(sData, 1, 2) + Mid(sData, 4, 2) + Mid(sData, 7, 4) + "_" + sHoraFile + "h"

        'Criar pasta em diretório
        System.IO.Directory.CreateDirectory(CaminhoCopia)
        CaminhoCopiaemUso = CaminhoCopia

        copiarDiretorio("\\172.16.4.71\clinicas4.0\", CaminhoCopia, False)

        Application.DoEvents()

        LbRegistros.Text = "Última Atualização em " + sDataAtual + " às " + sHora
        LbRegistros.Visible = True

        Application.DoEvents()

        Timer2.Interval = 20000  'Aguarda o tempo especificado para esconder a tela novamente

        'Habilitar novamente o Atualizador 
        Timer2.Enabled = True

    End Sub
    'Botão para ativar Backup automaticamente
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        'Desabilitar o Timer
        Timer3.Enabled = False

        Me.Show()

        Application.DoEvents()

        Dim sData As String = Mid(Now, 1, 10)
        Dim sHora As String = Mid(Now, 12, 5)  'Pegar a Hora Atual
        Dim sHoraFile = Replace(sHora, ":", "_")
        Dim CaminhoCopia = "C:\Backup\" + "SYSCOM_" + Mid(sData, 1, 2) + Mid(sData, 4, 2) + Mid(sData, 7, 4) + "_" + sHoraFile + "h"

        'Criar pasta em diretório
        System.IO.Directory.CreateDirectory(CaminhoCopia)
        CaminhoCopiaemUso = CaminhoCopia


        copiarDiretorio("\\172.16.4.71\clinicas4.0\", CaminhoCopia, False)

        Application.DoEvents()

        LbRegistros.Text = "Última Atualização em " + sDataAtual + " às " + sHora
        LbRegistros.Visible = True

        Application.DoEvents()

        Timer2.Interval = 20000  'Aguarda o tempo especificado para esconder a tela novamente

        'Habilitar novamente o Atualizador 
        Timer2.Enabled = True

    End Sub
End Class
