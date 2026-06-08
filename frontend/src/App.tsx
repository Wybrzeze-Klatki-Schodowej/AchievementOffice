import {BrowserRouter, Routes, Route} from 'react-router-dom';
import AppLayout from "./layouts/AppLayout";
// import LoginPage from "./pages/LoginPage";
import './App.css';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import { useEffect, useState } from 'react';
import { checkAuth } from './api/LoginApi';
import ProfilePage from './pages/ProfilePage';
import AdminUsersPage from './pages/AdminUsersPage';
import NotificationDetailsPage from './pages/NotificationDetailsPage';
import GroupsPage from './pages/GroupsPage';
import GroupPage from './pages/GroupPage';

function App() {
  const [isLoggedIn, setLoggedIn] = useState(false);

  useEffect(() => {
    checkLoggedIn();
  }, []);

  const checkLoggedIn = async () => {
    const isLoggedIn = await checkAuth();
    setLoggedIn(isLoggedIn);
  }

  return (
    <BrowserRouter>
      <Routes>
        <Route 
          path="/login" 
          element={<LoginPage onLogin={() => checkLoggedIn()} />}
        />
        <Route 
          path="/register" 
          element={<RegisterPage />} 
        />
        <Route 
          path="/" 
          element={ isLoggedIn ? <AppLayout /> : <LoginPage onLogin={() => checkLoggedIn()} />}
        >
          <Route 
            path="/users/:userId" 
            element={ isLoggedIn ? <ProfilePage /> : <LoginPage onLogin={() => checkLoggedIn()} />} 
                  />
                  <Route
                      path="/admin/users"
                      element={isLoggedIn ? <AdminUsersPage /> : <LoginPage onLogin={() => checkLoggedIn()} />}
                  />
          <Route
            path="/groups"
            element={isLoggedIn ? <GroupsPage /> : <LoginPage onLogin={() => checkLoggedIn()} />}
          />
          <Route
            path="/groups/:groupId"
            element={isLoggedIn ? <GroupPage /> : <LoginPage onLogin={() => checkLoggedIn()} />}
          />
        </Route>
        <Route 
          path="/verification-requests/:requestId"
          element={
            isLoggedIn
              ? <NotificationDetailsPage />
              : <LoginPage onLogin={() => checkLoggedIn()} />
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
