﻿using Gor.Devices;
using Gor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    Sensor k;

    Logger logger = new Logger(); 

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSeleziona_Click(object sender, EventArgs e)
    {
        if (DropDownList1.SelectedIndex == 0)
        {
            lblUnitàDiMisura.Text = "Gradi";
            lblNomeSensore.Text = "DS-1822";
            lblInterfaccia.Text = "1-Wire";
            // creazione del temometro conoscendo l'ID: 
            //k = new Temperature_DS1822("Temperatura aria", false, "48-02034234", logger); 
            // creazione del termometro con identificazione automatica del codice
            k = new Temperature_DS1822("Temperatura aria", false, logger); 

        }
        else if (DropDownList1.SelectedIndex == 1)
        {
            lblUnitàDiMisura.Text = "Percentuale";
            lblNomeSensore.Text = "HIH-4000";
            lblInterfaccia.Text = "1-Wire";
            k = new Humidity_Air_HIH4000("Air humidity", false, new Adc_MCP3208(), 1, logger);
        }
        else if (DropDownList1.SelectedIndex == 2)
        {
            lblUnitàDiMisura.Text = "Percentuale";
            lblNomeSensore.Text = "YL-69-YL-38";
            lblInterfaccia.Text = "1-Wire";
            k = new Humidity_Terrain_YL69YL38("Terrain Humidity", false, new Adc_MCP3208(), 1, logger);
        }
        else if (DropDownList1.SelectedIndex == 3)
        {
            lblUnitàDiMisura.Text = "[lx]";
            lblNomeSensore.Text = "Fotoresistor";
            lblInterfaccia.Text = "###";
            k = new Light_PhotoResistor("Temperature", false, new Adc_MCP3208(), 1, logger);
        }
    }
    protected void btnAvvia_Click(object sender, EventArgs e)
    {
        k.StartCalibration();
    }
    protected void btnPunto_Click(object sender, EventArgs e)
    {
        //k.
        //txtPuntoInput.Text = k.CalibrationPoint(double.Parse(txtPuntoInput.Text));
    }
    protected void btnChiusura_Click(object sender, EventArgs e)
    {
        k.EndCalibration();
    }
    protected void btnAbort_Click(object sender, EventArgs e)
    {

    }
}