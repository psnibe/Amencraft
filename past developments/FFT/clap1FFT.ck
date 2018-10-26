SndBuf2 amen;
FFT fft[2];
IFFT ifft[2];

amen.chan(0) => fft[0] =^ ifft[0] => dac.chan(0);
amen.chan(1) => fft[1] =^ ifft[1] => dac.chan(1);
me.dir()+"/audiobin/amen/simple_amen.wav" => amen.read;

512 => fft[0].size;
512 => fft[1].size;
0.5 => amen.gain;
1 => amen.loop;

while(true) 
{
    ifft[0].upchuck();
    ifft[1].upchuck();
    5 :: ms => now;
}