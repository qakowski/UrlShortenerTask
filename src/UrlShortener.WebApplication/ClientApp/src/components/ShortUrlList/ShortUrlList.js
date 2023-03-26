import React, { useState, useEffect } from 'react';
import ShortUrlListItem from '../ShortUrlListItem/ShortUrlListItem';
import useLazyLoading from '../../hooks/useLazyLoading';
import { fetchData } from '../../api.js';
import styles from './ShortUrlsList.module.scss';

const ShortUrlsList = ({ page, setPage, refresh, setRefresh }) => {
  const [data, setData] = useState([]);
  useLazyLoading(() => fetchData(page, 50), page, setData, data, refresh, [page]);

 useEffect(() => {
    const fetchDataAndUpdate = async () => {
      const newData = await fetchData(0, 0);
      setData(newData);
    };

    fetchDataAndUpdate();
  }, [page, refresh]);

  const handleRefresh = async () => {
    setPage(0);
    setRefresh((prevRefresh) => !prevRefresh);
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
      <button className="refresh-button" onClick={handleRefresh}>Refresh</button>
    </div>
  );
};

export default ShortUrlsList;
