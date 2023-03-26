import React, { useState } from 'react';
import ShortUrlsList from './components/ShortUrlList/ShortUrlList';
import ShortUrlForm from './components/ShortUrlForm/ShortUrlForm';
import './App.css';

function App() {
  const [page, setPage] = useState(0);
  const [refresh, setRefresh] = useState(false);

  const handleShortUrlCreated = (_) => {
    setPage(0);
    setRefresh((prevRefresh) => !prevRefresh);
  };

  return (
    <div className="container">
      <div className="App">
        <ShortUrlForm onShortUrlCreated={handleShortUrlCreated} />
          <ShortUrlsList page={page} setPage={setPage} refresh={refresh} setRefresh={setRefresh}/>
      </div>
    </div>
  );
}

export default App;
