#pragma once

#include "stdafx.h"
#include "glext.h"
#include <vector>
#include <gl/GL.h>

#define DllExport(t) extern "C"  __declspec( dllexport ) t __cdecl

PFNGLBINDVERTEXARRAYPROC		glBindVertexArray;
PFNGLUSEPROGRAMPROC				glUseProgram;
PFNGLACTIVETEXTUREPROC			glActiveTexture;
PFNGLBINDSAMPLERPROC			glBindSampler;
PFNGLBINDBUFFERBASEPROC			glBindBufferBase;
PFNGLBINDBUFFERRANGEPROC		glBindBufferRange;
PFNGLBINDFRAMEBUFFERPROC		glBindFramebuffer;
PFNGLBLENDFUNCSEPARATEPROC		glBlendFuncSeparate;
PFNGLBLENDEQUATIONSEPARATEPROC	glBlendEquationSeparate;
PFNGLBLENDCOLORPROC				glBlendColor;
PFNGLSTENCILFUNCSEPARATEPROC	glStencilFuncSeparate;
PFNGLSTENCILOPSEPARATEPROC		glStencilOpSeparate;
PFNGLPATCHPARAMETERIPROC		glPatchParameteri;
PFNGLDRAWARRAYSINSTANCEDPROC	glDrawArraysInstanced;
PFNGLDRAWELEMENTSINSTANCEDPROC  glDrawElementsInstanced;
PFNGLVERTEXATTRIBPOINTERPROC	glVertexAttribPointer;
PFNGLUNIFORM1FVPROC				glUniform1fv;
PFNGLUNIFORM1IVPROC				glUniform1iv;
PFNGLUNIFORM2FVPROC				glUniform2fv;
PFNGLUNIFORM2IVPROC				glUniform2iv;
PFNGLUNIFORM3FVPROC				glUniform3fv;
PFNGLUNIFORM3IVPROC				glUniform3iv;
PFNGLUNIFORM4FVPROC				glUniform4fv;
PFNGLUNIFORM4IVPROC				glUniform4iv;
PFNGLUNIFORMMATRIX2FVPROC		glUniformMatrix2fv;
PFNGLUNIFORMMATRIX3FVPROC		glUniformMatrix3fv;
PFNGLUNIFORMMATRIX4FVPROC		glUniformMatrix4fv;

// enum holding the available instruction codes
typedef enum {
	BindVertexArray = 1,
	BindProgram = 2,
	ActiveTexture = 3,
	BindSampler = 4,
	BindTexture = 5,
	BindBufferBase = 6,
	BindBufferRange = 7,
	BindFramebuffer = 8,
	Viewport = 9,
	Enable = 10,
	Disable = 11,
	DepthFunc = 12,
	CullFace = 13,
	BlendFuncSeparate = 14,
	BlendEquationSeparate = 15,
	BlendColor = 16,
	PolygonMode = 17,
	StencilFuncSeparate = 18,
	StencilOpSeparate = 19,
	PatchParameter = 20,
	DrawElements = 21,
	DrawArrays = 22,
	DrawElementsInstanced = 23,
	DrawArraysInstanced = 24,
	Clear = 25,
	BindImageTexture = 26,
	ClearColor = 27,
	ClearDepth = 28,
	GetError = 29,
	BindBuffer = 30,
	VertexAttribPointer = 31,
	VertexAttribDivisor = 32,
	EnableVertexAttribArray = 33,
	DisableVertexAttribArray = 34,
	Uniform1fv = 35,
	Uniform1iv = 36,
	Uniform2fv = 37,
	Uniform2iv = 38,
	Uniform3fv = 39,
	Uniform3iv = 40,
	Uniform4fv = 41,
	Uniform4iv = 42,
	UniformMatrix2fv = 43,
	UniformMatrix3fv = 44,
	UniformMatrix4fv = 45
} InstructionCode;

// enum controlling the current execution mode
typedef enum {
	None = 0x00000,
	RuntimeRedundancyChecks = 0x00001,
	RuntimeStateSorting = 0x00002
} VMMode;

// an instruction consists of a code and up to 5 arguments. 
typedef struct {
	InstructionCode Code;
	intptr_t Arg0;
	intptr_t Arg1;
	intptr_t Arg2;
	intptr_t Arg3;
	intptr_t Arg4;
} Instruction;

// a fragment consists of a substructured vector of instructions
typedef struct FragStruct {
	std::vector<std::vector<Instruction>> Instructions;
	struct FragStruct* Next;
} Fragment;

// runtime statistics
typedef struct {
	int TotalInstructions;
	int RemovedInstructions;
} Statistics;

DllExport(void) vmInit();
DllExport(Fragment*) vmCreate();
DllExport(void) vmDelete(Fragment* frag);
DllExport(bool) vmHasNext(Fragment* frag);
DllExport(Fragment*) vmGetNext(Fragment* frag);
DllExport(void) vmLink(Fragment* left, Fragment* right);
DllExport(void) vmUnlink(Fragment* left);
DllExport(int) vmNewBlock(Fragment* frag);
DllExport(void) vmClearBlock(Fragment* frag, int block);
DllExport(void) vmAppend1(Fragment* frag, int block, InstructionCode code, intptr_t arg0);
DllExport(void) vmAppend2(Fragment* frag, int block, InstructionCode code, intptr_t arg0, intptr_t arg1);
DllExport(void) vmAppend3(Fragment* frag, int block, InstructionCode code, intptr_t arg0, intptr_t arg1, intptr_t arg2);
DllExport(void) vmAppend4(Fragment* frag, int block, InstructionCode code, intptr_t arg0, intptr_t arg1, intptr_t arg2, intptr_t arg3);
DllExport(void) vmAppend5(Fragment* frag, int block, InstructionCode code, intptr_t arg0, intptr_t arg1, intptr_t arg2, intptr_t arg3, intptr_t arg4);
DllExport(void) vmClear(Fragment* frag);
DllExport(void) vmRunSingle(Fragment* frag);
DllExport(void) vmRun(Fragment* frag, VMMode mode, Statistics& stats);