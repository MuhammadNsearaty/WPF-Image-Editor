# WPF-Image-Editor
This project is for college project's 
a basic image editor application on windows
it provide's various functions on images:
- pen
- Rect
- circle
- cut
- crop
- selection
- layers system
- Resize
The Resize is the main tool in the application
because it provides resize to smaller shapes and super resolution
## Super Resolution
a simple example on this task that you have a 300x400 image you can make it 600x800 without losing details
and you can even make it 900x1200 (x2,x3...)
there is the traditional way and the AI way to scale the images
the traditional ways of interplolation like:
- Bicubic
- Bilinear
- Nearest neighbor

here you can choose the new width and height from a new windows that pop's up
The AI method used in this project is [EDSR](https://arxiv.org/abs/1707.02921)

it provides 2 options x2 and x3 and they alot of CPU and RAM resources the x3 on 4k images needs about 12GB of RAM
but the x2 works fine on 3\~4GB

the SR(Super Resolution) work's online and offline,
you can get the server [Image processing API](https://gitlab.com/Nitro963/image-processing-api.git)
and run it on some strong-specs PC,you can try the x3 on some high resolution images online it need's some time 4\~5 minutes
even if your presonal pc can't hold the x2 you can try it online

the server have cloud storing for images before and after SR and log's for all your uploads and downloads
make your own account on the server to save all ypur images
