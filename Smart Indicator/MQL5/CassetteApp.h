// //////////////////////////////////////////////////////////////////////////
// // Cassette application
// //////////////////////////////////////////////////////////////////////////

// #if !defined(AFX_CASSETTE_H__A6A4CE47_157C_4047_B55A_BA78A709DEB0__INCLUDED_)
// #define AFX_CASSETTE_H__A6A4CE47_157C_4047_B55A_BA78A709DEB0__INCLUDED_

// #if _MSC_VER > 1000
// #pragma once
// #endif // _MSC_VER > 1000

// #ifndef __AFXWIN_H__
// #error include 'stdafx.h' before including this file for PCH
// #endif

// #include "resource.h"       // main symbols
// #include "Functional.h"

// /////////////////////////////////////////////////////////////////////////////
// // CassetteApp:

// class CassetteApp : public CWinApp
// {
// public:
//     CassetteApp();

// public:
//     // Overrides
//     // ClassWizard generated virtual function overrides
//     //{{AFX_VIRTUAL(CassetteApp)
//     virtual BOOL InitInstance();
//     virtual int ExitInstance();
//     //}}AFX_VIRTUAL
//     enum SettingsParametersFlag
//     {
//         Setting_EnabledAutoRun = 0x0001,
//         Setting_EnabledHotkey = 0x0002,
//         Setting_EnabledHttpSrv = 0x0004,
//         Setting_EnabledScheme = 0x0008,
//         Setting_DatabasePath = 0x0010,
//         Setting_BackupPath = 0x0020,
//         Setting_WordslibPath = 0x0040,

//         Setting_AutoLogin = 0x0080,
//         Setting_SavePassword = 0x0100,

//         Setting_Username = 0x0200,
//         Setting_Password = 0x0400,
//     };

//     struct SettingsParameters
//     {
//         bool isEnabledAutoRun;   // �Ƿ�����������
//         bool isEnabledHotkey;    // �Ƿ����û��ȼ�
//         bool isEnabledHttpSrv;   // �Ƿ���HTTP����
//         bool isEnabledScheme;    // �Ƿ�ע��scheme
//         winplus::String databasePath;     // ���ݿ�·��
//         winplus::String backupPath;       // ����·��
//         winplus::String wordslibPath;     // �ʿ�·��

//         bool isAutoLogin;        // �Ƿ��Զ���¼
//         bool isSavePassword;     // �Ƿ񱣴�����

//         winplus::String username;         // ������û���(ec&base64����)
//         winplus::String password;         // ���������(ec&base64����)
//     };

//     struct CassetteSharedData
//     {
//         HWND hMainWnd; // �����ھ��
//     };
// protected:
//     winplus::SharedMemoryT<CassetteSharedData> m_sharedMem;//�����ڴ�
//     eiendb::Database * m_db;
//     winplus::WordsLib * m_wordslib;//�ʿ�

//     // ����/�رտ����Զ�����
//     void EnableAutoRun( bool isEnabled, bool isForce = false );
//     // ����/�ر�http����
//     void EnableHttpService( bool isEnabled );
//     // ����/�ر�Scheme
//     void EnableScheme( bool isEnabled );

//     // ʹ����ֻ�ܵ�������
//     // ��������һ������,���������ڴ�,����TRUE
//     // ���򽫴ӹ����ڴ��л�ȡ����������г���,����FALSE
//     BOOL DoSingletonRunning();
// public:
//     // �����ѵ�¼�û�
//     User m_loginedUser;
//     BOOL m_viaAutoLogin; // �Ƿ�ͨ���Զ���¼��ʽ��¼
//     SettingsParameters m_settings; // �������õĲ���

//     // ��ȡ�����ڴ�����
//     winplus::SharedMemoryT<CassetteSharedData> & GetSharedMemory() { return m_sharedMem; }
//     // ��ʼ�����ݱ�������
//     void InitDatabaseSchema();
//     // �����ݿ���Դ
//     void OpenDatabase();
//     // �ر����ݿ���Դ
//     void CloseDatabase();
//     // open wordslib
//     void OpenWordslib();
//     void CloseWordslib();
//     // close wordslib
//     // ��ȡ���ݿ�
//     eiendb::Database & GetDatabase() const { return *m_db; }
//     // get wordslib
//     winplus::WordsLib * GetWordslib() const { return m_wordslib; }
//     // ��������
//     bool BackupData( CString const & filename );
//     // �ָ�����
//     bool ResumeData( CString const & filename );
//     // ����ѡ������
//     // ��ini��ȡ���õ�������,flagָʾ��ʲô����
//     void LoadSettings( UINT flag = (UINT)-1 );
//     // �ӱ���д�����õ�ini��,flagָʾ��ʲô����
//     void SaveSettings( UINT flag = (UINT)-1 );
//     // ʹ������Ч��һЩѡ����Ҫ�ر�Ĺ��ܵ��ò�����Ч,flagָʾ��ʲô����
//     void DoSettings( UINT flag = (UINT)-1 );

//     //{{AFX_MSG(CassetteApp)
//     // NOTE - the ClassWizard will add and remove member functions here.
//     //    DO NOT EDIT what you see in these blocks of generated code !
//     //}}AFX_MSG
//     DECLARE_MESSAGE_MAP()
// };

// extern CassetteApp g_theApp;
// /////////////////////////////////////////////////////////////////////////////

// //{{AFX_INSERT_LOCATION}}
// // Microsoft Visual C++ will insert additional declarations immediately before the previous line.

// #endif // !defined(AFX_CASSETTE_H__A6A4CE47_157C_4047_B55A_BA78A709DEB0__INCLUDED_)
