16 => int channels;
SndBuf amen[channels];
SndBuf GGA[channels];
Pan2 pan[channels];
JCRev rev1[channels];
NRev rev2[channels];

for (0 => int i; i < channels; i++)
{
    amen[i] => rev1[i];
    me.dir()+"/audiobin/amen/simple_amen.wav" => amen[i].read;
    amen[i].samples() => amen[i].pos;
    
    GGA[i] => rev1[i];
    me.dir()+"/audiobin/random/GGAwimbledon.wav" => GGA[i].read;
    GGA[i].samples() => GGA[i].pos;
    
    rev1[i] => rev2[i] => pan[i] => dac;
}

amen[0].samples() => int all;
all/4 => int bar;
bar/4 => int beat;

GGA[0].samples() => int GGAfull;

fun void playamen(float gain, float rate, float tempo) {
    while (true)
    {
        Math.random2(0,channels-1) => int selector;
        (beat*selector) => amen[selector].pos;
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

spork ~ playamen(1.0, 1.0, beat);
spork ~ playGGA(1.0, 1.0, beat);

while( true ) 
    1::second => now;