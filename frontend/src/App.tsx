import {BrowserRouter, Routes, Route} from 'react-router-dom';
import AppLayout from "./layouts/AppLayout";
// import LoginPage from "./pages/LoginPage";
import './App.css';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import { useEffect, useState } from 'react';
import { checkAuth } from './api/LoginApi';
import ProfilePage from './pages/ProfilePage';

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
        <Route path="/login" element={<LoginPage onLogin={() => checkLoggedIn()} />}/>
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/" element={ isLoggedIn ? <AppLayout /> : <LoginPage onLogin={() => checkLoggedIn()} />}>
          <Route path="/" element={<ProfilePage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
