// our fixed convolution kernal (impulse response)
SndBuf s => FFT ffth => blackhole;  
me.dir()+"/audiobin/clap1.wav" => s.read; // whatever you like (caution of length!!)
2 => int fftSize;
while (fftSize < s.samples()) 
    2 *=> fftSize;           // next highest power of two
fftSize => int windowSize;   // this is windowsize, only apply to signal blocks
windowSize/2 => int hopSize; // this can any whole fraction of windowsize
2 *=> fftSize;               // zero pad by 2x factor (for convolve)
// our input signal, replace adc with anything you like
adc => Gain input => FFT fftx => blackhole;  // input signal
IFFT outy => dac;            // our output
fftSize => ffth.size => fftx.size => outy.size; // sizes
Windowing.hann(windowSize) => fftx.window;
//   <<< s.samples(), fftSize >>>;
windowSize::samp => now;     // load impulse response into h
ffth.upchuck() @=> UAnaBlob H; // spectrum of fixed impulse response
s =< ffth =< blackhole;      // don't need impulse resp signal anymore

complex Z[fftSize/2];
1000 => input.gain;          // fiddle with this how you like/need

while (true)  {
    fftx.upchuck() @=> UAnaBlob X; // spectrum of input signal
    
    // multiply spectra bin by bin (complex for free!):
    for(0 => int i; i < fftSize/2; i++ ) {
        fftx.cval(i) * H.cval(i) => Z[i];	
    }    
    outy.transform( Z );      // take ifft
    hopSize :: samp => now;   // and do it all again
}