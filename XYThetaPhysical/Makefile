
# https://www.cs.colostate.edu/~cs157/LectureMakefile.pdf

# CC = gcc
# CFLAGS = -Wall
# LDFLAGS =
# OBJFILES = Motor.o program.o
# TARGET = test

# all: $(TARGET)

# $(TARGET): $(OBJFILES)
# 	$(CC) $(CFLAGS) -o $(TARGET) $(OBJFILES) $(LDFLAGS)

# clean:

# rm -f $(OBJFILES) $(TARGET) *~

# https://stackoverflow.com/questions/22545029/makefile-how-to-correctly-include-header-file-and-its-directory

# CXX=g++

# INC_DIR = Compiled\ Binary
# SRC_DIR = Source\ Files
# OBJ_DIR = Object
# CFLAGS  = -c -Wall -I.
# SRCS = $(SRC_DIR)/Motor.cpp $(SRC_DIR)/program.cpp
# OBJS = $(OBJ_DIR)/Motor.o $(OBJ_DIR)/program.o
# # Note: The above will soon get unwieldy.
# # The wildcard and patsubt commands will come to your rescue.

# DEPS = $(INC_DIR)/libev3dev.a
# # Note: The above will soon get unwieldy.
# # You will soon want to use an automatic dependency generator.

# all: $(OBJS)

# $(OBJ_DIR)/%.o: $(SRC_DIR)/%.cpp
# 	$(CXX) $(CFLAGS) -c $< -o $@

# $(OBJ_DIR)/Trace.o: $(DEPS)

# clean:
# 	rm -rf *o all

OBJ_DIR = obj
LIB_DIR = lib
INC_DIR = inc
SRC_DIR = src
cpps = $(wildcard $(SRC_DIR)/*.cpp) $(wildcard $(SRC_DIR)/*/*.cpp)
objects = $(patsubst $(SRC_DIR)/%.o,$(OBJ_DIR)/%.o,$(patsubst %.cpp,%.o,$(cpps)))
CXX = arm-linux-gnueabi-g++
# CPPFLAGS = -c -Wall -I. -MD -MP
CPPFLAGS = -c -Wall -I$(INC_DIR) -MD -MP

all : $(objects)
		$(CXX) -o XYTheta $(objects) 
#$(LIB_DIR)/libev3dev.a

$(OBJ_DIR)/%.o: $(SRC_DIR)/%.cpp 
	@mkdir -p $(dir $@)
	$(CXX) $(CPPFLAGS) -c $< -o $@

# $(OBJ_DIR)/Motor.o : $(SRC_DIR)/Motor.cpp
# 		$(CXX) -o $(OBJ_DIR)/Motor.o -c $(SRC_DIR)/Motor.cpp

# $(OBJ_DIR)/Program.o : $(SRC_DIR)/Program.cpp
# 		$(CXX) -o $(OBJ_DIR)/Program.o -c $(SRC_DIR)/Program.cpp

.PHONY : clean
clean :
		rm -r $(OBJ_DIR)

.PHONY : cleanD
cleanD :
		find obj -name '*.d' -type f -delete

-include $(OBJ_DIR)/%.d

#rm
#https://stackoverflow.com/questions/8025766/makefile-auto-dependency-generation
#http://www.microhowto.info/howto/automatically_generate_makefile_dependencies.html

# CPPFLAGS += -MD -MP

# SRC = $(wildcard *.c)

# hello_world: $(SRC:%.c=%.o)
# 	$(LD) -o $@ $^

# -include $(SRC:%.c=%.d)