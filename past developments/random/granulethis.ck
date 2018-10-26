2500 => int sampLength;
12 => int silos;
SndBuf2 grainery[silos];
0 => int sampCount;
0 => int selector;

for (0 => int i; i < silos; i++)
{
    me.dir()+"/audiobin/custom palette/loops/"+(i+4)+".wav" => grainery[i].read;
    grainery[i] => dac;
    0 => grainery[i].pos;
    0.0 => grainery[i].gain;
    1 => grainery[i].loop;
}

fun void blend()
{
    while(true)
    {
        sampCount % silos => selector;
        if (selector == 0)
        {
            0.5 => grainery[silos-1].gain;
            1.0 => grainery[0].gain;
            0.5 => grainery[1].gain;
 
            sampLength::samp => now;
            sampCount++;
            
            0.0 => grainery[silos-1].gain;
            0.0 => grainery[0].gain;
            0.0 => grainery[1].gain;
        }
        if (selector == silos-1)
        {
            0.5 => grainery[silos-2].gain;
            1.0 => grainery[silos-1].gain;
            0.5 => grainery[0].gain;
            
            sampLength::samp => now;
            sampCount++;
            
            0.0 => grainery[silos-2].gain;
            0.0 => grainery[silos-1].gain;
            0.0 => grainery[0].gain;
        }
        else
        {
            if (selector > 0)
            {
                0.5 => grainery[selector-1].gain;
                1.0 => grainery[selector].gain;
                0.5 => grainery[selector+1].gain;
                
                sampLength::samp => now;
                sampCount++;
                
                0.0 => grainery[selector-1].gain;
                0.0 => grainery[selector].gain;
                0.0 => grainery[selector+1].gain;
            }
        }
    }
}

blend();