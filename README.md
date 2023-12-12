# GEA_folder_repo
GEA folder exam report
For my folder exam I decided to make a simulation utilizing AI. Initially I sought to implement a neural network leveraging deep Q learning/ reinforcement learning from the ground up. 
This proved cumbersome without resorting to prebuilt python libraries or ML agents in Unity (OpenAI package). Instead, I opted for a genetic algorithm implementation, to be close to the code and have a greater learning outcome.

To learn the functionalities of neural networks I utilized the coding trains video series on the subject, starting by building a simple perceptron and later a feed forward neural network. 
Combining this with a genetic algorithm, I created cars with randomized parameters starting the networks nodes starting values and weights of its connections. 
Which in turn gave the network data to evaluate and train on, and further on combing the best cars, in my case the cars who made it furthest along the track, to produce better and better offspring/ cars who make it through the track the fastest.

In this endeavor I had great support from ArtzSamuel project on github, which use a similar implementation for cars in 2d. This project was a great help regarding combining the neural network and the genetic algorithm. Many of the concepts and core logic are derived from this project but modified to make it work in a 3d context.

The cars further utilize the rigidbody system of unity to apply gravity to the cars, keeping them grounded, acceleration is clamped to work in the XZ plane, in the direction the car is facing. 

Besides the concepts discussed the project utilizes several of the design patterns learn throughout the course to keep code clean, and components communicating across their respective classes.

In order to tinker with the simulation one can adjust the parameters of the genetic manager game object. 
Here one can adjust how many cars participate in the simulation in each generation, how many generations will be simulated before the algorithm resets (meaning, a new set of randomized parameters are given to the cars network, and the evolution process resets), 
whether or not the best cars genomes should be saved to a file before the algorithm resets, and what kind of selection method should be used for evolution (elitist selection or stochastic sampling, if elitist is disabled).

Furthermore, the parameters of the car can be adjusted, in terms of its maxspeed, acceleration and turnspeed/rate, by changing the car game objects movement components respective fields. There is also support for changing the maximum delay the cars may have between each checkpoint in the car controller component.

References:
Coding training, neural network video series:
https://www.youtube.com/watch?v=XJ7HLz9VYz0&list=PLRqwX-V7Uu6aCibgK1PTWWu9by6XFdCfh
Applying evolutionary artificial neural networks, ArtzSamuel, github:
https://github.com/ArztSamuel/Applying_EANNs/tree/master
