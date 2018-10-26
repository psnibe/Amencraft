16 => int channels;
SndBuf amen[channels];
SndBuf GGA[channels];
Pan2 pan[channels];
JCRev rev1[channels];
NRev rev2[channels];
0 => int allsamples;

for (0 => int i; i < channels; i++)
{
    amen[i] => rev1[i];
    me.dir()+"/audiobin/amen/loops/"+i+".wav" => amen[i].read;
    amen[i].samples() +=> allsamples;
    amen[i].samples() => amen[i].pos;
    
    GGA[i] => rev1[i];
    me.dir()+"/audiobin/random/GGAwimbledon.wav" => GGA[i].read;
    GGA[i].samples() => GGA[i].pos;

    rev1[i] => rev2[i] => pan[i] => dac;
    0.0 => rev1[i].mix;
    0.0 => rev2[i].mix;
}

16 /=> allsamples;
allsamples/2 => int bar;
bar/4 => int beat;

GGA[0].samples() => int GGAfull;

fun void playamen(float gain, float rate, float tempo) {
    while (true)
    {
        Math.random2(0,channels-1) => int selector;
        beat*(Math.random2(0,7)) => amen[selector].pos;
        gain => amen[selector].gain;
        rate => amen[selector].rate;
        Math.random2f(-0.75,0.75) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        tempo::samp => now;
        amen[selector].samples() => amen[selector].pos;
    }
}

fun void playGGA(float gain, float rate, float tempo) {
    while (true)
    {
        Math.random2(0,channels-1) => int selector;
        Math.random2(0,GGAfull) => GGA[selector].pos;
        gain => GGA[selector].gain;
        rate => GGA[selector].rate;
        Math.random2f(-0.75,0.75) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        tempo::samp => now;
        GGA[selector].samples() => GGA[selector].pos;
    }
}


spork ~ playamen(1.0, 1.0, bar);
spork ~ playamen(0.75, 1.0, bar/2);

spork ~ playamen(0.5, 1.0, beat);
spork ~ playamen(0.25, 1.0, beat/2);

spork ~ playGGA(2.0, 1.0, bar);
spork ~ playGGA(2.0, 1.0, bar/2);

while( true ) 
    1::second => now;