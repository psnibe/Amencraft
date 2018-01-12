SndBuf clap1 => FFT fft =^ IFFT ifft => dac;
me.dir()+"/audiobin/clap1.wav" => clap1.read;
8 => fft.size;
0.5 => clap1.gain;
1 => clap1.rate;
while(true) 
{
    0 => clap1.pos;
    ifft.upchuck();
    500.0 :: ms => now;
}