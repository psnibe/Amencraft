SndBuf clap1 => Pan2 pan => dac;

me.dir()+"/audiobin/clap1.wav" => clap1.read;

while(true) 
{
    Math.random2f(0.1,1.0) => clap1.gain;
    Math.random2f(-1.0,1.0) => pan.pan;
    Math.random2f(0.2,1.8) => clap1.rate;
    0 => clap1.pos;
    500.0 :: ms => now;
}