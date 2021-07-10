import { observer } from 'mobx-react-lite';
import { useEffect, useState } from 'react';
import InfiniteScroll from 'react-infinite-scroller';
import { Grid, List, Loader } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { PagingParams } from '../../../app/models/pagination';
import { useStore } from '../../../app/stores/store';
import ActivityFilters from './ActivityFilters';
import ActivityList from './ActivityList';

function ActivityDashboard() {
  const { activityStore } = useStore();
  const {
    loadActivities,
    activityRegistry,
    pagination,
    setPagination,
    setPagingParams,
  } = activityStore;

  const [loadingNext, setLoadingNext] = useState<boolean>(false);

  function handleGetNext() {
    setLoadingNext(true);
    setPagingParams(new PagingParams(pagination!.currentPage + 1));
    loadActivities().then(() => setLoadingNext(false));
  }

  useEffect(() => {
    if (activityRegistry.size <= 1) loadActivities();
  }, [loadActivities, activityRegistry.size]);

  // return the whole loading page when loading
  if (activityStore.loadingInitial && !loadingNext) {
    return <LoadingComponent content="Loading activities" />;
  }

  return (
    <Grid>
      <Grid.Column width="10">
        <List>
          <InfiniteScroll
            pageStart={0}
            loadMore={handleGetNext}
            hasMore={
              !loadingNext &&
              !!pagination &&
              pagination.currentPage < pagination.totalPages
            }
            initialLoad={false}
          >
            <ActivityList />
          </InfiniteScroll>
        </List>
      </Grid.Column>

      <Grid.Column width="6">
        <ActivityFilters />
      </Grid.Column>

      <Grid.Column width="10">
        <Loader active={loadingNext} />
      </Grid.Column>
    </Grid>
  );
}

export default observer(ActivityDashboard);
