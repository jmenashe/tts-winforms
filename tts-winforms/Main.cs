﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.Speech.Synthesis.TtsEngine;
using System.Diagnostics;

namespace Tts.WinForms
{
    public partial class Main : Form
    {
        SpeechSynthesizer speechSynthesizer;
        SelectableText selectableTextInstance;
        SsmlOptionsController ssmlOptionsController;
        AudioNameGenerator audioNameGenerator;
        bool isSsmlMarkupInUse = true;
        int audioCount = 0;
        


        public Main()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadInstalledVoices();
            UpdateRateLabel();
            UpdateVolumeLabel();
            Notify("");

            selectableTextInstance = new SelectableText(textToRead, textBoxLabel);
            ssmlOptionsController = new SsmlOptionsController(resetSsmlMarkupLangListBox, ssmlMarkupLangListBox, textToRead, ssmlOptionBreadcrumbTxt);
            audioNameGenerator = new AudioNameGenerator(fileNameTxt);
        }

        private void sliderRate_Scroll(object sender, EventArgs e)
        {
            UpdateRateLabel();
        }

        private void sliderVolume_Scroll(object sender, EventArgs e)
        {
            UpdateVolumeLabel();
        }
        
        private void stopBtn_Click(object sender, EventArgs e)
        {
            StopSynthesizer();
        }

        private void listenBtn_Click(object sender, EventArgs e)
        {
            ReInitSynthesizer();

            if( isSsmlMarkupInUse) { speechSynthesizer.SpeakSsmlAsync(GetSsmlText()); }
            else { speechSynthesizer.SpeakAsync(GetTextToRead()); }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            ReInitSynthesizer();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += "\\Audio Files\\";
            Directory.CreateDirectory(path); // Create directory on Desktop

            var fileName = audioNameGenerator.GetFileName();
            var audioName = fileName + ".wav";

            speechSynthesizer.SetOutputToWaveFile(path + audioName);

            if (isSsmlMarkupInUse) { speechSynthesizer.SpeakSsmlAsync(GetSsmlTextToSave()); }
            else { speechSynthesizer.SpeakAsync(GetTextToSave()); }

            Notify("Saved as: " + audioName);
        }

        #region Local helpers
        private void LoadInstalledVoices()
        {
            speechSynthesizer = new SpeechSynthesizer();
            var voices = speechSynthesizer.GetInstalledVoices();
            foreach (InstalledVoice voice in voices)
            {
                VoiceInfo infoVoice = voice.VoiceInfo;
                cmbVoice.Items.Add(infoVoice.Name);
            }
            
            cmbVoice.SelectedItem = speechSynthesizer.Voice.Name;
            speechSynthesizer.Dispose();
        }
        private void ReInitSynthesizer()
        {
            Notify("");
            StopSynthesizer();
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(SynthesizerSpeakStarted);
            speechSynthesizer.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(SynthesizerSpeakCompleted);
            ConfigureSynthesizerParams();
        }
        private void ConfigureSynthesizerParams()
        {
            speechSynthesizer.SelectVoice(cmbVoice.SelectedItem as string);
            speechSynthesizer.Volume = Convert.ToInt32(sliderVolume.Value);
            speechSynthesizer.Rate = Convert.ToInt32(sliderRate.Value);
        }
        private void StopSynthesizer()
        {
            try
            {
                if (speechSynthesizer != null && speechSynthesizer.State == SynthesizerState.Speaking)
                {
                    speechSynthesizer.Dispose();
                }
            }
            catch (Exception ex) { }
        }
        private void UpdateRateLabel()
        {
            rateLbl.Text = "Rate: " + sliderRate.Value.ToString();
        }
        private void UpdateVolumeLabel()
        {
            volumeLbl.Text = "Volume: " + sliderVolume.Value.ToString();
        }
        private void SynthesizerSpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            stopBtn.Enabled = true;
        }
        private void SynthesizerSpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            stopBtn.Enabled = false;
        }
        private string GetSsmlText()
        {
            return SsmlConverter.ConvertTextIntoSSML(GetTextToRead(), speechSynthesizer);
        }
        private string GetSsmlTextToSave()
        {
            return SsmlConverter.ConvertTextIntoSSML(GetTextToSave(), speechSynthesizer);
        }
        private string GetTextToRead()
        {
            return textToRead.SelectionLength > 0 ? textToRead.SelectedText : textToRead.Text;
        }
        private string GetTextToSave()
        {
            return textToRead.SelectionLength > 0 ? textToRead.SelectedText : textToRead.Text;
        }
        private void Notify(string text)
        {
            notificationLbl.Text = text;
        }
        #endregion

        private void selectActiveTextBtn_Click(object sender, EventArgs e)
        {
            // Button text: "Show Original"
            if (selectableTextInstance.IsSelectedDisplayedOnly())
            {
                selectableTextInstance.DisplayOriginal();
            }
            // Button text: "Isolate text"
            else
            { 
                selectableTextInstance.IsolateSelectedText();
            }

            selectActiveTextBtn.Text = selectableTextInstance.IsSelectedDisplayedOnly() ? 
                "Show Original" 
                : "Isolate text";
        }

        private void toggleSsmlMarkupUseBtn_Click(object sender, EventArgs e)
        {
            isSsmlMarkupInUse = !isSsmlMarkupInUse;

            ssmlOptionsController.ToggleVisibility(isSsmlMarkupInUse);
            toggleSsmlMarkupUseBtn.Text = isSsmlMarkupInUse ? "Basic mode" : "Advanced mode";
        }

        private void ssmlDocLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.microsoft.com/en-us/cortana/skills/speech-synthesis-markup-language");
        }
    }
}
