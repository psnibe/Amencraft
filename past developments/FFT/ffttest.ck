// our patch
SndBuf clap1 => FFT fft => blackhole;
// synthesize
IFFT ifft => dac;

// set parameters
1024 => fft.size;
0.5 => clap1.gain;

// use this to hold contents
complex s[fft.size()/2];

// control loop
while( true )
{
    0 => clap1.pos;
    // take fft
    fft.upchuck();
    // get contents
    fft.spectrum( s );
    // take ifft
    ifft.transform( s );  
    // advance time
    fft.size()::samp => now;
}