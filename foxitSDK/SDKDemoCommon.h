﻿//#ifndef _FOXITSDK_H_
//#define _FOXITSDK_H_
#pragma once

/** Common header files. */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <string>
#include <iostream>
#include <vector>
#include <locale.h>

/** Include header files of SDK. */
#include "../foxitSDK/include/fsdk.h"


namespace foxitSDK
{

	//PDF document handle. 
	public value struct FileHandle { int64 pointer; /* The value of the pointer to document */ };

	//PDF document handle. 
	public value struct DocHandle { int64 pointer; /* The value of the pointer to document */ };

	//PDF page handle. 
	public value struct PageHandle { int64 pointer; /* The value of the pointer to page */ };

	public value struct TextPageHandle { int64 pointer; };

	public value struct MatrixHandle { int64 pointer; };

	public value struct TextSearchHandle { int64 pointer; };

	public value struct UD_LPVOID { int64 pointer; };

	
	//PDF textpage handle.
	//public value struct TextPageHandle { int64 pointer; /* The value of the pointer to textpage */ };

	//PDF textsearch handle.
	//public value struct TextSearchHandle { int64 pointer; /* The value of the pointer to textsearch */ };
	
	//DIB format Flags. 
	public enum class PixelFormat
	{
		BGRx = 2		    // 4 bytes per pixel, byte order: Blue, Green, Red, not used. 
	};

	// A class to present the DIB data created from SDK.
	public ref class PixelSource sealed
	{
	public:
		PixelSource() {}

		property PixelFormat								Format;			// The DIB format defined with enum Foxit::General::PixelFormat. 
		property Windows::Storage::Streams::IBuffer^		PixelBuffer;	// The DIB pixel source buffer. 
		property int32										Width;			// The width of DIB. 
		property int32										Height;			// The height of DIB. 
	};


	//Implementation of callback functions in CMy_File.
	class CMy_FileReadWrite
	{
	public:
		CMy_FileReadWrite();
		virtual ~CMy_FileReadWrite();
		void	CreateFileRead(Windows::Storage::StorageFile^ pFile, int nFileSize);
		void	Release();
		int		GetSize();
		void	SetSize(int nSize);
		bool	ReadBlock(void* buffer, int offset, size_t size);
		bool	WriteBlock(const void* buffer, int offset, size_t size);
		void	ReleaseTempWriteBuffer();

		Platform::Collections::Vector<Platform::Object^>^ GetFileBuffer();
	private:
		Windows::Storage::StorageFile^						m_pFile;
		int													m_iFileSize;
		Platform::Collections::Vector<Platform::Object^>^	m_Buffer;
	};

	//Inherited class of FSCRT_FILEHANDLER
	class CMy_File : public FSCRT_FILEHANDLER
	{
	public:
		CMy_File();
		~CMy_File();
		void InitFileHandle(CMy_FileReadWrite* pFileHandle);
		void ReleaseFileHandle();

		//Inherited callback funtions.
		static void g_PrivateRelease(FS_LPVOID clientData);
		static FS_DWORD g_PrivateGetSize(FS_LPVOID clientData);
		static FS_RESULT g_PrivateReadBlock(FS_LPVOID clientData, FS_DWORD offset, FS_LPVOID buffer, FS_DWORD size);
		static FS_RESULT g_PrivateWriteBlock(FS_LPVOID clientData, FS_DWORD offset, FS_LPCVOID buffer, FS_DWORD size);
		static FS_RESULT g_PrivateFlush(FS_LPVOID clientData);
		static FS_RESULT g_PrivateTruncate(FS_LPVOID clientData, FS_DWORD size);

	private:
	};





	public ref class FSDK_Document sealed
	{
	public:
		FSDK_Document();

		//Open PDF document
		Windows::Foundation::IAsyncOperation<FS_RESULT>^	OpenDocumentAsync(Windows::Storage::StorageFile^ pdfFile, int32 nFileSize);

		//Load PDF page and also parse page.
		FS_RESULT	LoadPageSync(int32 iPageIndex);

		////////////////////////////////////
		FS_RESULT	LoadTextPage(PageHandle page, TextPageHandle * textPage);
		FS_RESULT   LoadMatrix(PageHandle page, int iStartX, int iStartY, int iSizeX, int iSizeY, int iRotation, MatrixHandle * matrix);
		FS_RESULT	LoadAnnot(PageHandle page);
		FS_RESULT	unloadAnnot(PageHandle page);
		FS_RESULT	getCharIndex(TextPageHandle textPage, MatrixHandle matrix, double x, double y, int * charIndex);
		FS_RESULT	getCharNum(TextPageHandle textPage, int * count);
		FS_RESULT	startSelectText(TextPageHandle textPage, int startIndex, int charNum);
		FS_RESULT	getRectNum(int * Num);
		FS_RESULT	getRect(int rectIndex, MatrixHandle matrix, int * x, int * y, int * z, int * w);
		Platform::String^	getChar(TextPageHandle textPage, int startIndex, int charNum, int * errCode);
		FS_RESULT	endSelectText();

		FS_RESULT addAnnot(PageHandle page, TextPageHandle textPage, MatrixHandle matrix, Platform::String^ word);

		///////////////////////////////////////////
		//Release member parameters.
		void		ReleaseResource();

		//Render page asynchronously and convert SDK bitmap data to IRandomAccessStreamWithContentType.
		Windows::Foundation::IAsyncOperation<Windows::Storage::Streams::IRandomAccessStreamWithContentType^>^ \
			RenderPageAsync(PixelSource^ pxsrc, int iStartX, int iStartY, int iSizeX, int iSizeY, int iRotation, PageHandle page);

		//Save current PDF file to another PDF file.
		Windows::Foundation::IAsyncOperation<bool>^ SaveAsDocument(Windows::Storage::StorageFile^ file);

		property FileHandle     m_hFile;      // The file handle. 
		property DocHandle      m_hDoc;       // The doc handle. 
		property PageHandle		m_hPage;      // The page handle. 

	private:
		~FSDK_Document();

		//Render page to SDK bitmap and get its data.
		bool GetRenderBitmapData(PixelSource^ pxsrc, int iStartX, int iStartY, int iSizeX, int iSizeY, int iRotation, PageHandle page);

		//Save as PDF file.
		Windows::Foundation::Collections::IObservableVector<Object^>^	SaveAsPDF();

		//Write data in byte to file.
		concurrency::task<Windows::Storage::Streams::IOutputStream^>	WriteByteToFile(Windows::Storage::StorageFile^ file, Windows::Foundation::Collections::IObservableVector<Object^>^ fileBytes);

		CMy_FileReadWrite*	m_pFileReader;
		CMy_File*			m_pFileStream;

		FSPDF_TEXTSELECTION textselection = NULL;
	};









	public ref class Inherited_PDFFunction sealed
	{
	public:
		Inherited_PDFFunction();

		FS_RESULT	My_Page_Clear(PageHandle page);
		FS_RESULT	My_TextPage_Clear(TextPageHandle textPage);
		FS_RESULT	My_Matrix_Clear(MatrixHandle matrix);
		FS_RESULT	My_TextSearch_Clear(TextSearchHandle textSearch);

		FS_RESULT	My_Page_GetSize(PageHandle page, FS_FLOAT* width, FS_FLOAT* height);

		FS_INT32    My_Doc_CountPages(DocHandle doc);

		//FS_RESULT hiliteText(PageHandle page, float startX, float startY, float endX,float endY, int iStartX, int iStartY, int iSizeX, int iSizeY, int iRotation);

		Platform::String^ GetWordFromLocation(TextPageHandle itextpage, MatrixHandle imatrix, float x, float y, int type);

		FS_RESULT   SearchInCurrentPage(Platform::String^ searchText, TextPageHandle textPageH);

		/**
		* @brief	Initialize Foxit SDK library. Also initialize PDF module and load system font.
		*
		* @return	::FSCRT_ERRCODE_SUCCESS for success.<br>
		*			For more error codes, please refer to macro definitions <b>FSCRT_ERRCODE_XXX</b>.
		*
		* @note	When use Foxit SDK library, the first step is to initialize library.
		*/
		FS_RESULT	FSDK_Initialize();

		/**
		* @brief	Finalize PDF module and Foxit SDK library.
		*
		* @note	Before exit from application, user need call this function to exit PDF module and destroy Foxit SDK library.
		*			This function calls FSCRT_Library_DestroyMgr function to finalize manager created in FSDK_Initialize.
		*/
		void		FSDK_Finalize();

	/*private:
		FSCRT_ANNOT selectAnnot = NULL;
		int selectStartIndex = 0;
		int selectEndIndex = 0;*/
	};

	public ref class Search_PDFFunction sealed
	{
	public:

		Search_PDFFunction(DocHandle doc, Platform::String^ searchText);

		void Finish_Search();
		FS_RESULT My_StartSearch(int iPageIndex, int* resultPageIndex);

		FS_RESULT FindNext(int* resultPageIndex);

		FS_RESULT FindPrevious(int* resultPageIndex);

		FS_RESULT FindAllResult();

		FS_RESULT HandlePage(int iPageIndex);

	private:
		~Search_PDFFunction();

		FS_INT32          pageCount;
		FS_INT32          currentPage;
		FSCRT_DOCUMENT    Doc;
		FSPDF_TEXTPAGE    textPage;
		FSCRT_BSTR        searchPattern;
		FSPDF_TEXTSEARCH  textSearch;
	};

	// Convert platform string to FSCR_BSTR
	//std::shared_ptr<FSCRT_BSTR>		ConvertToFoxitString(Platform::String^ str);

	// Show error log in message dialog.
	void		ShowErrorLog(Platform::String^ errorContent, Windows::UI::Popups::UICommandInvokedHandler^ returnInvokedHandler = nullptr,
		bool bSDKError = false, FS_RESULT errorcode = FSCRT_ERRCODE_ERROR);

	/**
	* @brief	Render page to SDK bitmap.
	*
	* @param[in]	page		Handle to a valid <b>FSCRT_PAGE</b> object.
	* @param[in]	bmpWidth	The width of bitmap.
	* @param[in]	bmpHeight	The height of bitmap.
	* @param[in]	iStartX		Used by FSPDF_Page_GetMatrix.
	* @param[in]	iStartY		Used by FSPDF_Page_GetMatrix.
	* @param[in]	iSizeX		Used by FSPDF_Page_GetMatrix.
	* @param[in]	iSizeY		Used by FSPDF_Page_GetMatrix.
	* @param[in]	iRotation	Page rotation value. Use one of macro definitions <b>FSCRT_PAGEROTATION_XXX</b>.
	* @param[out]	renderBmp	Used to receive SDK bitmap object, to which the page has already been rendered.
	*
	* @return	::FSCRT_ERRCODE_SUCCESS for success.<br>
	*			For more error codes, please refer to macro definitions <b>FSCRT_ERRCODE_XXX</b>.
	*/
	FS_RESULT	FSDK_PageToBitmap(FSCRT_PAGE page, int bmpWidth, int bmpHeight, int iStartX, int iStartY, int iSizeX, int iSizeY, int iRotation, FSCRT_BITMAP *renderBmp);

	/**
	* @brief	Get SDK bitmap's data.
	*
	* @param[in]	bmp		SDK bitmap object, which's data is to be retrieved.
	* @param[out]	dib		Used to receive SDK bitmap data.
	*
	* @return	::FSCRT_ERRCODE_SUCCESS for success.<br>
	*			For more error codes, please refer to macro definitions <b>FSCRT_ERRCODE_XXX</b>.
	*/
	FS_RESULT	FSDK_GetSDKBitmapData(FSCRT_BITMAP bmp, PixelSource^ dib);


	/*
	*choose the english character (from a to z,or from A to Z)
	*result = 0 character is not valid
	*result = 2 character is Uppcase
	*result = 1 character is Lowercase
	*result = 3 character is '-'
	*result = 4 character is '\n'
	*/
	FS_RESULT   IsCharacterValid(char character);

	//ud_struct
	public value struct hiliteInfo_1 {
		double beginX;
		double beginY;
		double endX;
		double endY;
		PageHandle page;
	};
}
//#endif //_FOXITSDK_H_
