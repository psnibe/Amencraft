using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpatializeCenter: MonoBehaviour {

    public AudioMixer mixerWithChuck;
    private string spatialChuck;

    // Use this for initialization
    void Start() {

        spatialChuck = "spatial_chuck_center";
        Chuck.Manager.Initialize(mixerWithChuck, spatialChuck);

        Chuck.Manager.RunCode(spatialChuck,
            @"

            SndBuf2 fancytest;
            SndBuf2 delayedfancytest;
            Gain left;
            Gain right;
            Gain leftsampler;
            Gain rightsampler;
            Gain leftsum;
            Gain rightsum;
            DelayL leftdelay;
            DelayL rightdelay;
            Gain leftdelaygain;
            Gain rightdelaygain;
            float leftlevel;
            float rightlevel;
            float loudest;
            float leftdelaytime;
            float rightdelaytime;
            0.5 => float gainamount;
            1.0 => float maxdelay; // in milliseconds

            // spatializer input
            adc.chan(0) => leftsampler => left;
            adc.chan(1) => rightsampler => right;

            // soundfile input
            me.dir() + ""/basscutfancynails.wav"" => fancytest.read;
            me.dir() + ""/slowerfancynails.wav"" => delayedfancytest.read;
            11 => delayedfancytest.pos;
            1 => fancytest.loop;
            1 => delayedfancytest.loop;
            fancytest.chan(0) => left;
            fancytest.chan(1) => right;

            // multiply sample output with spatializer input
            3 => left.op;
            3 => right.op;

            // send to final sum gains
            left => leftsum  => dac.chan(0);
            right => rightsum  => dac.chan(1);
            gainamount => left.gain;
            gainamount => right.gain;

            // delay sidechain
            delayedfancytest.chan(0) => leftdelay => leftdelaygain => rightsum; // switch channels to
            delayedfancytest.chan(1) => rightdelay => rightdelaygain => leftsum; // normalize effect
            (maxdelay * 10)::ms => leftdelay.max => leftdelay.delay;
            (maxdelay * 10)::ms => rightdelay.max => rightdelay.delay;
            gainamount => leftdelaygain.gain;
            gainamount => rightdelaygain.gain;

            fun void amplitudeTracker() {
                while(true) {
                    leftsampler.last() => leftlevel => loudest;
                    rightsampler.last() => rightlevel;
                    if (rightlevel > loudest) { rightlevel => loudest; }
                    1::samp => now;
                }
            }

            fun void amplitudePrinter() {
                while(true) {
                    chout <= ""L: "" <= rightdelaytime; // remember,
                    chout <= IO.newline();
                    chout <= ""R: "" <= leftdelaytime; // it's switched
                    chout <= IO.newline();
                    100::ms => now;
                }
            }

            fun void delayDriver() {
                while(true) {
                    maxdelay * leftlevel => leftdelaytime;
                    maxdelay * rightlevel => rightdelaytime;
                    leftdelaytime::ms => leftdelay.delay;
                    rightdelaytime::ms => rightdelay.delay;
                    1::samp => now;
                }
            }

            fun void amplitudeDriver() {
                while(true) {
                    gainamount * loudest => leftdelaygain.gain => rightdelaygain.gain;
                    1::samp => now;
                }
            }

            spork ~ amplitudeTracker();
            spork ~ amplitudePrinter();
            spork ~ delayDriver();
            spork ~ amplitudeDriver();

            while( true )
                1::second => now;

            "
        );

    }
   
}
