import {BrowserRouter, Routes, Route} from 'react-router-dom';
import AppLayout from "./layouts/AppLayout";
// import LoginPage from "./pages/LoginPage";
import './App.css';
import LoginPage from './pages/LoginPage';
import { useState } from 'react';

function App() {
  const [isLoggedIn, setLoggedIn] = useState(false);

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />}/>
        <Route path="/" element={ isLoggedIn ? <AppLayout /> : <LoginPage onLogin={() => setLoggedIn(true)} />}>
          {/* <Route path="/" element={<LoginPage />}/> */}
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App
