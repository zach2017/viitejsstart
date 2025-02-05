import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import PublicPage from './pages/PublicPage';
import PrivatePage from './pages/PrivatePage';
import PopupBanner from './components/PopupBanner';
import ProtectedRoute from './auth/ProtectedRoute';
import Login from './auth/Login';

const App = () => {
  return (
    <Router>
      <PopupBanner />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/public" element={<PublicPage />} />
        <Route path="/login" element={<Login />} />
        <Route element={<ProtectedRoute />}>
        <Route path="/private" element={<PrivatePage />} />
        </Route>
      </Routes>
    </Router>
  );
};

export default App;
