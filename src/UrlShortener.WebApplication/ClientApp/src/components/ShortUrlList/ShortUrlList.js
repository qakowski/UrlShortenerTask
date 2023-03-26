import React, { useState, useEffect } from 'react';
import ShortUrlListItem from '../ShortUrlListItem/ShortUrlListItem';
import useLazyLoading from '../../hooks/useLazyLoading';
import { fetchData } from '../../api.js';
import styles from './ShortUrlsList.module.scss';

const ShortUrlsList = ({ page, setPage, refresh }) => {
  const [data, setData] = useState([]);
  const { loading } = useLazyLoading(() => fetchData(page, 50), [page, refresh]);

  useEffect(() => {
    const fetchDataAndUpdate = async () => {
      const newData = await fetchData(page, 50);
      setData(newData);
    };

    fetchDataAndUpdate();
  }, [page, refresh]);

  const handleRefresh = async () => {
    setPage(0);
    await fetchData(page, 50);
  };

  return (
    <div className={styles.container}>
      <h2>Short URLs</h2>
      <table className={styles.table}>
        <thead>
          <tr>
            <th>Original URL</th>
            <th>Short URL</th>
          </tr>
        </thead>
        <tbody>
          {data.map((urlObj) => (
            <ShortUrlListItem key={urlObj.shortUrl} originalUrl={urlObj.url} shortUrl={urlObj.shortUrl} />
          ))}
        </tbody>
      </table>
      {loading && <p>Loading...</p>}
      {!loading && <button className="load-more-button" onClick={handleRefresh}>Refresh</button>}
    </div>
  );
};

export default ShortUrlsList;
