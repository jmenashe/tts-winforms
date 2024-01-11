﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tts.WinForms
{
    class SsmlOptionsController
    {
        ListBox listBox;
        TextBox textToRead;
        Button resetButton;
        List<SsmlOption> options = new List<SsmlOption>();
        List<string> trackUserOptions = new List<string>();
        TextBox breadcrumbText;

        public SsmlOptionsController(Button resetButton, ListBox listBox, TextBox textBox, TextBox ssmlOptionBreadcrumbTxt)
        {
            this.listBox = listBox;
            this.textToRead = textBox;
            this.breadcrumbText = ssmlOptionBreadcrumbTxt;
            this.resetButton = resetButton;

            this.InitializeOptions();

            this.listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyDown);
            this.listBox.DoubleClick += new System.EventHandler(this.HandleDoubleClick);
            this.resetButton.Click += new System.EventHandler(this.resetSsmlMarkupLangListBox_Click);
            
            this.SetItems();
        }


        internal void ToggleVisibility(bool isVisible)
        {
            if (!isVisible)
            {
                trackUserOptions.Clear();
                this.SetItems();
            }
            breadcrumbText.Visible = isVisible;
            listBox.Visible = isVisible;
        }

        private void InitializeOptions()
        {
            this.options.Clear();

            {
                SsmlOption breakOption = new SsmlOption("break", "Break", OptionType.Insert);

                SsmlOption strengthOption = new SsmlOption("strength", "Strength");
                strengthOption.AddToChildren(new SsmlOption("x-weak"));
                strengthOption.AddToChildren(new SsmlOption("weak"));
                strengthOption.AddToChildren(new SsmlOption("medium"));
                strengthOption.AddToChildren(new SsmlOption("strong"));
                strengthOption.AddToChildren(new SsmlOption("x-strong"));

                SsmlOption timeOption = new SsmlOption("time", "Time");
                timeOption.AddToChildren(new SsmlOption("100ms", "100 ms"));
                timeOption.AddToChildren(new SsmlOption("200ms", "200 ms"));
                timeOption.AddToChildren(new SsmlOption("300ms", "300 ms"));
                timeOption.AddToChildren(new SsmlOption("400ms", "400 ms"));
                timeOption.AddToChildren(new SsmlOption("500ms", "500 ms"));
                timeOption.AddToChildren(new SsmlOption("600ms", "600 ms"));
                timeOption.AddToChildren(new SsmlOption("700ms", "700 ms"));
                timeOption.AddToChildren(new SsmlOption("800ms", "800 ms"));
                timeOption.AddToChildren(new SsmlOption("900ms", "900 ms"));
                timeOption.AddToChildren(new SsmlOption("1000ms", "1000 ms"));
                timeOption.AddToChildren(new SsmlOption("1500ms", "1500 ms"));
                timeOption.AddToChildren(new SsmlOption("2000ms", "2000 ms"));


                breakOption.AddToChildren(timeOption);
                breakOption.AddToChildren(strengthOption);

                this.options.Add(breakOption);
            }
            
            {
                SsmlOption prosodyOption = new SsmlOption("prosody", "Prosody (volume, rate, duration, pitch)", OptionType.Wrap);

                SsmlOption pitchOption = new SsmlOption("pitch", "Pitch");
                pitchOption.AddToChildren(new SsmlOption("x-low"));
                pitchOption.AddToChildren(new SsmlOption("low"));
                pitchOption.AddToChildren(new SsmlOption("medium"));
                pitchOption.AddToChildren(new SsmlOption("high"));
                pitchOption.AddToChildren(new SsmlOption("x-high"));
                pitchOption.AddToChildren(new SsmlOption("default"));

                SsmlOption rateOption = new SsmlOption("rate", "Rate");
                rateOption.AddToChildren(new SsmlOption("x-slow"));
                rateOption.AddToChildren(new SsmlOption("slow"));
                rateOption.AddToChildren(new SsmlOption("medium"));
                rateOption.AddToChildren(new SsmlOption("fast"));
                rateOption.AddToChildren(new SsmlOption("x-fast"));
                rateOption.AddToChildren(new SsmlOption("default"));

                SsmlOption durationOption = new SsmlOption("duration", "Duration");
                durationOption.AddToChildren(new SsmlOption("250ms", "250 ms"));
                durationOption.AddToChildren(new SsmlOption("500ms", "500 ms"));
                durationOption.AddToChildren(new SsmlOption("750ms", "750 ms"));
                durationOption.AddToChildren(new SsmlOption("1000ms", "1000 ms"));
                durationOption.AddToChildren(new SsmlOption("1250ms", "1250 ms"));
                durationOption.AddToChildren(new SsmlOption("1500ms", "1500 ms"));
                durationOption.AddToChildren(new SsmlOption("2000ms", "2000 ms"));

                SsmlOption volumeOption = new SsmlOption("volume", "Volume");
                volumeOption.AddToChildren(new SsmlOption("0", "0%"));
                volumeOption.AddToChildren(new SsmlOption("25", "25%"));
                volumeOption.AddToChildren(new SsmlOption("50", "50%"));
                volumeOption.AddToChildren(new SsmlOption("75", "75%"));
                volumeOption.AddToChildren(new SsmlOption("95", "95%"));
                volumeOption.AddToChildren(new SsmlOption("100", "100%"));


                prosodyOption.AddToChildren(volumeOption);
                prosodyOption.AddToChildren(rateOption);
                prosodyOption.AddToChildren(durationOption);
                prosodyOption.AddToChildren(pitchOption);

                this.options.Add(prosodyOption);
            }
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ResetOptions();
            }
        }

        private void HandleDoubleClick(object sender, EventArgs e)
        {
            SsmlOption selectedOption = null;
            
            foreach (SsmlOption option in GetActiveOptions())
            {
                if(listBox.SelectedItem == option.text)
                {
                    selectedOption = option;
                    break;
                }
            }

            if( selectedOption == null) { return; }
            
            HandleSelectNewOption(selectedOption);
        }

        private void HandleSelectNewOption(SsmlOption option)
        {
            trackUserOptions.Add(option.text);

            if( option.isFinalOption())
            {
                var tags = option.GetTags();
                
                var wrapStart = textToRead.SelectionStart;
                var wrapEnd = textToRead.SelectionStart + textToRead.SelectionLength;

                string newText = textToRead.Text;
                newText = newText.Insert(wrapEnd, tags[1]);
                newText = newText.Insert(wrapStart, tags[0]);

                textToRead.Text = newText;
                ResetOptions();
            }

            this.SetItems();
        }

        private void resetSsmlMarkupLangListBox_Click(object sender, EventArgs e)
        {
            trackUserOptions.RemoveAt(trackUserOptions.Count - 1);
            this.SetItems();
        }

        private void ResetOptions()
        {
            trackUserOptions.Clear();
            this.SetItems();
        }

        private void SetItems()
        {
            this.listBox.Items.Clear();
            var breadText = "";

            foreach(SsmlOption option in GetActiveOptions())
            {
                this.listBox.Items.Add(option.text);
            }

            for(var it= 0; it< trackUserOptions.Count; it++)
            {
                if (breadText!="") { breadText += " > "; }
                breadText += Truncate(trackUserOptions[it], 10);
            }
            breadcrumbText.Text = breadText;

            resetButton.Visible = trackUserOptions.Count > 0;
        }

        private string Truncate(string text, int max)
        {
            if (text.Length > max + 3)
            {
                return text.Substring(0, max) + "...";
            }
            return text;
        }

        private List<SsmlOption> GetActiveOptions()
        {
            List<SsmlOption> FindChildrenOfItem(List<SsmlOption> targetOptions, string item)
            {
                foreach (SsmlOption option in targetOptions)
                {
                    if (item == option.text)
                    {
                        return option.children;
                    }
                }
                return null;
            }

            var currentOptions = options;
            foreach (string selectedItem in trackUserOptions)
            {
                currentOptions = FindChildrenOfItem(currentOptions, selectedItem);
            }
            return currentOptions;
        }
    }
}
