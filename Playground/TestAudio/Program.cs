﻿
class Program
{

    static void Main()
    {
        //MididateiFunktionen.MididateiAbspielen("Some great song.mid", "TimGM6mb.sf2");
        SchreibeMidi.erstellespezifischMidi("AkkordTest");
        MididateiFunktionen.MididateiAuslesen("akkord_with_instrument.mid");
        
    }
}