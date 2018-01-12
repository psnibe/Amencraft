SndBuf instruments[4];

instruments[0] => dac; //kick
instruments[1] => dac; //snare
instruments[2] => dac; //snareside
instruments[3] => dac; //hihat

me.dir()+"/audiobin/kick.wav" => instruments[0].read;
me.dir()+"/audiobin/snare.wav" => instruments[1].read;
me.dir()+"/audiobin/snareside.wav" => instruments[2].read;
me.dir()+"/audiobin/hihat.wav" => instruments[3].read;

adc => Gain input => FFT fft => blackhole;
250 => input.gain; 
1024 => fft.size;
Windowing.hamming(fft.size()) => fft.window;

[0.0,0.0,0.0,0.0] @=> float maxes[];
0 => int trigger;
         
while (true)  
{
    fft.upchuck() @=> UAnaBlob blob;
    0 => float sum;
    0 => float avg;
    for (0 => int i; i < 4; i++)
    {
        for( i*fft.size()/8 => int j; j < i+1*fft.size()/8 ; j++)
        {
            blob.fvals()[j] +=> sum;
            sum/fft.size()/8 => avg;
            if( avg/2 > maxes[0] )
            {
                if (trigger == 1) 
                {
                    0 => instruments[0].pos;
                    0 => trigger;
                }
            }
            else
            {
                1 => trigger;
            }
            avg => maxes[0];
        }
    }
    (fft.size()*8)::samp => now;
}