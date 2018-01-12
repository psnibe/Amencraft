16 => int channels;
SndBuf hihat[channels];
Pan2 pan[channels];
JCRev rev1[channels];
NRev rev2[channels];
300::ms => dur bar;

for (0 => int i; i < channels; i++)
{
    hihat[i] => rev1[i] => pan[i] => rev2[i] => dac;
    me.dir()+"/audiobin/hihats/"+i+".wav" => hihat[i].read;
    hihat[i].samples() => hihat[i].pos;
}

fun void play(float gain, float rate, dur tempo) {
    while (true)
    {
        Math.random2(0,channels-1) => int selector;
        0 => hihat[selector].pos;
        gain => hihat[selector].gain;
        rate => hihat[selector].rate;
        Math.random2f(-0.50,0.50) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.15) => rev2[selector].mix;
        tempo => now;
    }
}

spork ~ play(1.0, 0.75, bar*2);
spork ~ play(0.75, 1.0, bar);
spork ~ play(0.5, 1.25, bar/2);
spork ~ play(0.25, 1.5, bar/4);
         
while( true ) 
    1::second => now;