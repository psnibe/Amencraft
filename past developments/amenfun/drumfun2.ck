16 => int channels;
SndBuf hihat[channels];
SndBuf snare[channels];
SndBuf kick[channels];
Pan2 pan[channels];
JCRev rev1[channels];
NRev rev2[channels];
300::ms => dur bar;

for (0 => int i; i < channels; i++)
{
    hihat[i] => rev1[i];
    me.dir()+"/audiobin/hihats/"+i+".wav" => hihat[i].read;
    hihat[i].samples() => hihat[i].pos;
    
    snare[i] => rev1[i];
    me.dir()+"/audiobin/snares/"+i+".wav" => snare[i].read;
    snare[i].samples() => snare[i].pos;
    
    kick[i] => rev1[i];
    me.dir()+"/audiobin/kicks/"+i+".wav" => kick[i].read;
    kick[i].samples() => kick[i].pos;
    
    rev1[i] => rev2[i] => pan[i] => dac;
}

fun void playhihat(float gain, float rate, dur tempo) {
    while (true)
    {
        Math.random2(0,channels-1) => int selector;
        0 => hihat[selector].pos;
        gain => hihat[selector].gain;
        rate => hihat[selector].rate;
        Math.random2f(-0.75,0.75) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        tempo => now;
    }
}

fun void playsnare(float gain, float rate, dur tempo) {
    while (true)
    {
        Math.random2(0,channels-1) => int selector;
        snare[selector].samples() => int samples;
        0 => snare[selector].pos;
        gain => snare[selector].gain;
        rate => snare[selector].rate;
        rate / samples => float ratedecrement;
        rate => float currentrate;
        tempo / samples => dur timeincrement;
        dur totaltime;
        Math.random2f(-0.5,0.5) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        while (totaltime < tempo)
        {
            ratedecrement -=> currentrate;
            currentrate => snare[selector].rate;
            timeincrement +=> totaltime;
            timeincrement => now;
        }
    }
}

fun void playkick(float gain, float rate, dur tempo) {
    while (true)
    {
        Math.random2(0,channels-1) => int selector;
        0 => kick[selector].pos;
        gain => kick[selector].gain;
        rate => kick[selector].rate;
        Math.random2f(-0.5,0.5) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        tempo => now;
    }
}

spork ~ playhihat(1.0, 0.75, bar*1.5);
spork ~ playhihat(0.75, 1.0, bar);
spork ~ playhihat(0.5, 1.25, bar/2);
spork ~ playhihat(0.25, 1.5, bar/4);

spork ~ playkick(1.0, 1.0, bar);
spork ~ playkick(0.5, 1.25, bar/2);

bar => now;

spork ~ playsnare(1.0, 1.0, bar*2);
         
while( true ) 
    1::second => now;